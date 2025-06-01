using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocationById;

public class GetLocationByIdHandler
    : IQueryHandler<GetLocationByIdQuery, LocationDTO>
{
    private readonly AbstractValidator<GetLocationByIdQuery> _validator;
    private readonly ILogger<GetLocationByIdHandler> _logger;
    private readonly ILocationRepository _locationRepository;

    public GetLocationByIdHandler(
        AbstractValidator<GetLocationByIdQuery> validator,
        ILogger<GetLocationByIdHandler> logger,
        ILocationRepository locationRepository)
    {
        _validator = validator;
        _logger = logger;
        _locationRepository = locationRepository;
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

        var id = Id<Location>.Create(query.Id);
        var result = await _locationRepository.GetByIdAsync(id, cancellationToken);
        if (result.IsFailure)
            return result.Errors;

        return LocationDTO.FromDomainEntity(result.Value);
    }
}
