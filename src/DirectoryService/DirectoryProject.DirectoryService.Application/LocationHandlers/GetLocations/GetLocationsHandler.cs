using Dapper;
using DirectoryProject.DirectoryService.Application.Shared.Database;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Extensions;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocations;

public class GetLocationsHandler
    : IQueryHandler<GetLocationsQuery, FilteredListDTO<LocationDTO>>
{
    private readonly AbstractValidator<GetLocationsQuery> _validator;
    private readonly ILogger<GetLocationsHandler> _logger;
    private readonly IDBConnectionFactory _connectionFactory;

    public GetLocationsHandler(
        AbstractValidator<GetLocationsQuery> validator,
        ILogger<GetLocationsHandler> logger,
        IDBConnectionFactory connectionFactory)
    {
        _validator = validator;
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<FilteredListDTO<LocationDTO>>> HandleAsync(
        GetLocationsQuery query,
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
            FROM diretory_service.locations
            WHERE is_active = true
            """);
        if (string.IsNullOrEmpty(query.Search) == false)
        {
            totalCountBuilder.Append(" AND");
            totalCountBuilder.AddTextSearchCondition("name", query.Search);
        }

        var totalCount = await connection.ExecuteScalarAsync<int>(
            totalCountBuilder,
            _logger,
            cancellationToken);

        var selectBuilder = new CustomSQLBuilder(
            """
            SELECT id, name, address, time_zone
            FROM diretory_service.locations
            WHERE is_active = true
            """);
        if (string.IsNullOrEmpty(query.Search) == false)
        {
            selectBuilder.Append(" AND");
            selectBuilder.AddTextSearchCondition("name", query.Search);
        }

        selectBuilder
            .ApplySorting(new Dictionary<string, bool>
            {
                { "name", true },
            })
            .ApplyPagination(query.Page, query.Size);

        var selectResult = await connection.QueryAsync<LocationDTO>(
            selectBuilder,
            _logger,
            cancellationToken);

        return new FilteredListDTO<LocationDTO>(
            query.Page,
            query.Size,
            selectResult,
            totalCount);
    }
}
