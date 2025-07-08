using Core.Abstractions;
using Core.Database;
using Dapper;
using DirectoryProject.DirectoryService.Application.DTOs;
using SharedKernel;
using FluentValidation;
using Microsoft.Extensions.Logging;
using static Dapper.SqlMapper;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocationById;

public class GetLocationByIdHandler
    : IQueryHandler<GetLocationByIdQuery, LocationDTO>
{
    private readonly AbstractValidator<GetLocationByIdQuery> _validator;
    private readonly ILogger<GetLocationByIdHandler> _logger;
    private readonly IDBConnectionFactory _connectionFactory;

    public GetLocationByIdHandler(
        AbstractValidator<GetLocationByIdQuery> validator,
        ILogger<GetLocationByIdHandler> logger,
        IDBConnectionFactory connectionFactory)
    {
        _validator = validator;
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<LocationDTO>> HandleAsync(
        GetLocationByIdQuery query,
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

        var parameters = new DynamicParameters();
        parameters.Add("@id", query.Id);

        using var connection = _connectionFactory.Create();

        var result = await connection.QueryFirstOrDefaultAsync<LocationDTO>(new CommandDefinition(
            commandText: """
                SELECT id, name, address, time_zone
                FROM directory_service.locations
                WHERE id = @id and is_active = true
                """,
            parameters: parameters,
            cancellationToken: cancellationToken));
        if (result is null)
            return ErrorHelper.General.NotFound(query.Id);

        _logger.LogInformation("Location with id {id} red", query.Id);

        return result;
    }
}
