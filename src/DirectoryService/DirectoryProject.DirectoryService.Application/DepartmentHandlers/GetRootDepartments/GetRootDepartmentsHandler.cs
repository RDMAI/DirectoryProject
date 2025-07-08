using Core.Abstractions;
using Core.Database;
using Dapper;
using DirectoryProject.DirectoryService.Application.DTOs;
using DirectoryProject.DirectoryService.Application.Helpers;
using DirectoryProject.FileService.Communication;
using DirectoryProject.FileService.Contracts.Dto;
using DirectoryProject.FileService.Contracts.Requests;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedKernel;
using System.Linq;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetRootDepartments;

public class GetRootDepartmentsHandler
    : IQueryHandler<GetRootDepartmentsQuery, FilteredListDTO<DepartmentTreeDTO>>
{
    private readonly AbstractValidator<GetRootDepartmentsQuery> _validator;
    private readonly ILogger<GetRootDepartmentsHandler> _logger;
    private readonly IDBConnectionFactory _connectionFactory;
    private readonly IFileService _fileService;

    public GetRootDepartmentsHandler(
        AbstractValidator<GetRootDepartmentsQuery> validator,
        ILogger<GetRootDepartmentsHandler> logger,
        IDBConnectionFactory connectionFactory,
        IFileService fileService)
    {
        _validator = validator;
        _logger = logger;
        _connectionFactory = connectionFactory;
        _fileService = fileService;
    }

    public async Task<Result<FilteredListDTO<DepartmentTreeDTO>>> HandleAsync(
        GetRootDepartmentsQuery query,
        CancellationToken cancellationToken = default)
    {
        // command validation
        var validatorResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validatorResult.IsValid)
        {
            return validatorResult.Errors
                .Select(e => Error.Deserialize(e.ErrorMessage))
                .ToList();
        }

        using var connection = _connectionFactory.Create();

        var totalCountBuilder = new CustomSQLBuilder(
            """
            SELECT count(id)
            FROM directory_service.departments
            WHERE is_active = true AND depth = 0
            """);

        var totalCount = await connection.ExecuteScalarAsync<int>(
            totalCountBuilder,
            _logger,
            cancellationToken);

        var multipleSelectBuilder = new CustomSQLBuilder(
            """
            SELECT id, name, parent_id, path, depth, children_count,
                CONCAT(logo->>'fileid', '|', logo->>'location') AS LogoUrl
            FROM directory_service.departments
            WHERE is_active = true AND depth = 0
            ORDER BY name
            LIMIT @limit OFFSET @offset;

            SELECT id, name, parent_id, path, depth, children_count,
                CONCAT(logo->>'fileid', '|', logo->>'location') AS LogoUrl
            FROM (SELECT
                id, name, parent_id, path, depth, children_count, logo,
                ROW_NUMBER () OVER (
                    PARTITION BY parent_id
                    ORDER BY name)
                    AS r_number
                FROM directory_service.departments
                WHERE is_active = true AND depth = 1)
            WHERE r_number <= @prefetch;
            """);
        multipleSelectBuilder.Parameters.Add("@offset", (query.Page - 1) * query.Size);
        multipleSelectBuilder.Parameters.Add("@limit", query.Size);
        multipleSelectBuilder.Parameters.Add("@prefetch", query.Prefetch);

        var queryResult = await connection.QueryMultipleAsync(
            multipleSelectBuilder,
            _logger,
            cancellationToken);

        var roots = queryResult.Read<DepartmentTreeDTO>().AsList();
        var children = queryResult.Read<DepartmentDTO>();

        var getUrlsResponse = await ConvertLogoObjectsToUrls(
            roots,
            children,
            cancellationToken);
        if (getUrlsResponse.IsFailure)
            return getUrlsResponse.Errors;

        // in memory mapping with dictionary is faster than json aggregation
        // (according to https://medium.com/@nelsonciofi/the-best-way-to-store-and-retrieve-complex-objects-with-dapper-5eff32e6b29e)
        var lookupMap = new Dictionary<Guid, int>();

        for (int i = 0; i < roots.Count; i++)
            lookupMap.Add(roots[i].Id, i);

        foreach (var child in children)
        {
            if (lookupMap.TryGetValue(child.ParentId!.Value, out int index))
                roots[index].Children.Add(child);
        }

        // roots' children now filled
        return new FilteredListDTO<DepartmentTreeDTO>(
            Page: query.Page,
            Size: query.Size,
            Total: totalCount,
            Data: roots);
    }

    /// <summary>
    /// SQL select query returns DepartmentDTO with LogoUrl containing file id and bucket name separated by '|'.
    /// To get actual urls we call file service
    /// </summary>
    /// <param name="departmentsWithLogo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<UnitResult> ConvertLogoObjectsToUrls(
        IEnumerable<DepartmentTreeDTO> rootDepartmentsWithLogo,
        IEnumerable<DepartmentDTO> childrenDepartmentsWithLogo,
        CancellationToken cancellationToken = default)
    {
        int rootCount = rootDepartmentsWithLogo.Count();
        int childrenCount = childrenDepartmentsWithLogo.Count();

        // fill up the list of logo requests for both parents and children
        var logoLocations = new List<FileLocation>(rootCount + childrenCount);

        foreach (var dep in rootDepartmentsWithLogo)
        {
            if (dep.LogoUrl.Length == 1) // this happens if the logo is null
            {
                dep.LogoUrl = string.Empty;
                continue;
            }

            var fileLocation = DepartmentSqlHelper.ConvertStringToFileLocation(dep.LogoUrl);
            logoLocations.Add(fileLocation);
        }

        foreach (var dep in childrenDepartmentsWithLogo)
        {
            if (dep.LogoUrl.Length == 1) // this happens if the logo is null
            {
                dep.LogoUrl = string.Empty;
                continue;
            }

            var fileLocation = DepartmentSqlHelper.ConvertStringToFileLocation(dep.LogoUrl);
            logoLocations.Add(fileLocation);
        }

        var fileResponse = await _fileService.GetDownloadURLAsync(
            new GetDownloadURLsRequest(logoLocations),
            cancellationToken);
        if (fileResponse.IsFailure)
            return fileResponse.Errors;

        // mapping back gotten responses to their departments (they should be in the same order)
        int counter = 0;
        foreach (var dep in rootDepartmentsWithLogo)
        {
            if (string.IsNullOrEmpty(dep.LogoUrl))
                continue;

            dep.LogoUrl = fileResponse.Value.URLs[counter].Url;
            counter++;
        }

        foreach (var dep in childrenDepartmentsWithLogo)
        {
            if (string.IsNullOrEmpty(dep.LogoUrl))
                continue;

            dep.LogoUrl = fileResponse.Value.URLs[counter].Url;
            counter++;
        }

        return UnitResult.Success();
    }
}
