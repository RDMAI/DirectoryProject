using Core.Abstractions;
using Core.Database;
using Dapper;
using DirectoryProject.DirectoryService.Application.DTOs;
using SharedKernel;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetRootDepartments;

public class GetRootDepartmentsHandler
    : IQueryHandler<GetRootDepartmentsQuery, FilteredListDTO<DepartmentTreeDTO>>
{
    private readonly AbstractValidator<GetRootDepartmentsQuery> _validator;
    private readonly ILogger<GetRootDepartmentsHandler> _logger;
    private readonly IDBConnectionFactory _connectionFactory;

    public GetRootDepartmentsHandler(
        AbstractValidator<GetRootDepartmentsQuery> validator,
        ILogger<GetRootDepartmentsHandler> logger,
        IDBConnectionFactory connectionFactory)
    {
        _validator = validator;
        _logger = logger;
        _connectionFactory = connectionFactory;
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
            FROM diretory_service.departments
            WHERE is_active = true AND depth = 0
            """);

        var totalCount = await connection.ExecuteScalarAsync<int>(
            totalCountBuilder,
            _logger,
            cancellationToken);

        var multipleSelectBuilder = new CustomSQLBuilder(
            """
            SELECT id, name, parent_id, path, depth, children_count
            FROM diretory_service.departments
            WHERE is_active = true AND depth = 0
            ORDER BY name
            LIMIT @limit OFFSET @offset;

            SELECT *
            FROM (SELECT
                id, name, parent_id, path, depth, children_count,
                ROW_NUMBER () OVER (
                    PARTITION BY parent_id
                    ORDER BY name)
                    AS r_number
                FROM diretory_service.departments
                WHERE is_active = true AND depth = 1)
            WHERE r_number <= @prefetch;
            """);
        multipleSelectBuilder.Parameters.Add("@offset", (query.Page - 1) * query.Size);
        multipleSelectBuilder.Parameters.Add("@limit", query.Size);
        multipleSelectBuilder.Parameters.Add("@prefetch", query.Prefetch);

        var result = await connection.QueryMultipleAsync(
            multipleSelectBuilder,
            _logger,
            cancellationToken);

        var roots = result.Read<DepartmentTreeDTO>().AsList();
        var children = result.Read<DepartmentTreeDTO>();

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
}
