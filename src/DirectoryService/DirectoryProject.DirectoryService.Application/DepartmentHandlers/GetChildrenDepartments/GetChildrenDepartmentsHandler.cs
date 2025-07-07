using Core.Abstractions;
using Core.Database;
using DirectoryProject.DirectoryService.Application.DTOs;
using SharedKernel;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetChildrenDepartments;

public class GetChildrenDepartmentsHandler
    : IQueryHandler<GetChildrenDepartmentsQuery, FilteredListDTO<DepartmentDTO>>
{
    private readonly AbstractValidator<GetChildrenDepartmentsQuery> _validator;
    private readonly ILogger<GetChildrenDepartmentsHandler> _logger;
    private readonly IDBConnectionFactory _connectionFactory;

    public GetChildrenDepartmentsHandler(
        AbstractValidator<GetChildrenDepartmentsQuery> validator,
        ILogger<GetChildrenDepartmentsHandler> logger,
        IDBConnectionFactory connectionFactory)
    {
        _validator = validator;
        _logger = logger;
        _connectionFactory = connectionFactory;
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
            SELECT id, name, parent_id, path, depth, children_count
            FROM directory_service.departments
            WHERE is_active = true AND parent_id = @parent_id
            """);
        selectBuilder.Parameters.Add("@parent_id", query.ParentId);

        selectBuilder.ApplySorting(new Dictionary<string, bool> {
            { "name", true },
        }).ApplyPagination(query.Page, query.Size);

        var selectResult = await connection.QueryAsync<DepartmentDTO>(
            selectBuilder,
            _logger,
            cancellationToken);

        return new FilteredListDTO<DepartmentDTO>(
            query.Page,
            query.Size,
            selectResult,
            totalCount);
    }
}
