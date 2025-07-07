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

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetChildrenDepartments;

public class GetChildrenDepartmentsHandler
    : IQueryHandler<GetChildrenDepartmentsQuery, FilteredListDTO<DepartmentDTO>>
{
    private readonly AbstractValidator<GetChildrenDepartmentsQuery> _validator;
    private readonly ILogger<GetChildrenDepartmentsHandler> _logger;
    private readonly IDBConnectionFactory _connectionFactory;
    private readonly IFileService _fileService;

    public GetChildrenDepartmentsHandler(
        AbstractValidator<GetChildrenDepartmentsQuery> validator,
        ILogger<GetChildrenDepartmentsHandler> logger,
        IDBConnectionFactory connectionFactory,
        IFileService fileService)
    {
        _validator = validator;
        _logger = logger;
        _connectionFactory = connectionFactory;
        _fileService = fileService;
    }

    public async Task<Result<FilteredListDTO<DepartmentDTO>>> HandleAsync(
        GetChildrenDepartmentsQuery query,
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
            WHERE is_active = true AND parent_id = @parent_id
            """);
        totalCountBuilder.Parameters.Add("@parent_id", query.ParentId);

        var totalCount = await connection.ExecuteScalarAsync<int>(
            totalCountBuilder,
            _logger,
            cancellationToken);

        var selectBuilder = new CustomSQLBuilder(
            """
            SELECT id, name, parent_id, path, depth, children_count,
                CONCAT(logo->>'fileid', '|', logo->>'location') AS LogoUrl
            FROM directory_service.departments
            WHERE is_active = true AND parent_id = @parent_id
            """);
        selectBuilder.Parameters.Add("@parent_id", query.ParentId);

        selectBuilder.ApplySorting(new Dictionary<string, bool> {
            { "name", true },
        }).ApplyPagination(query.Page, query.Size);

        var queryResult = await connection.QueryAsync<DepartmentDTO>(
            selectBuilder,
            _logger,
            cancellationToken);

        // splitted for readability
        var getLogoUrlsResult = await ConvertLogoObjectsToUrls(queryResult, cancellationToken);
        if (getLogoUrlsResult.IsFailure)
            return getLogoUrlsResult.Errors;

        return new FilteredListDTO<DepartmentDTO>(
            query.Page,
            query.Size,
            queryResult,
            totalCount);
    }

    /// <summary>
    /// SQL select query returns DepartmentDTO with LogoUrl containing file id and bucket name separated by '|'.
    /// To get actual urls we call file service
    /// </summary>
    /// <param name="departmentsWithLogo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<UnitResult> ConvertLogoObjectsToUrls(
        IEnumerable<DepartmentDTO> departmentsWithLogo,
        CancellationToken cancellationToken = default)
    {
        int length = departmentsWithLogo.Count();

        var logoLocations = new List<FileLocation>(departmentsWithLogo.Count());
        foreach (var dep in departmentsWithLogo)
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

        int counter = 0;
        foreach (var dep in departmentsWithLogo)
        {
            if (string.IsNullOrEmpty(dep.LogoUrl))
                continue;

            dep.LogoUrl = fileResponse.Value.URLs[counter].Url;
            counter++;
        }

        return UnitResult.Success();
    }
}
