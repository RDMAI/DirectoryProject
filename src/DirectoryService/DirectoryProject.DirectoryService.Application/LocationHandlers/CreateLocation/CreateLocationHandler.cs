using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.CreateLocation;

public class CreateLocationHandler
    : ICommandHandler<CreateLocationCommand, LocationDTO>
{
    private readonly AbstractValidator<CreateLocationCommand> _validator;
    private readonly ILogger<CreateLocationHandler> _logger;
    private readonly ILocationRepository _locationRepository;

    public CreateLocationHandler(
        AbstractValidator<CreateLocationCommand> validator,
        ILogger<CreateLocationHandler> logger,
        ILocationRepository locationRepository)
    {
        _validator = validator;
        _logger = logger;
        _locationRepository = locationRepository;
    }

    public async Task<Result<LocationDTO>> HandleAsync(
        CreateLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        // command validation
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validatorResult.IsValid)
        {
            return validatorResult.Errors
                .Select(e => Error.Deserialize(e.ErrorMessage))
                .ToList();
        }

        var name = LocationName.Create(command.Name).Value!;
        var isNameUnique = await _locationRepository.IsNameUniqueAsync(name, cancellationToken);
        if (isNameUnique.IsFailure)
            return isNameUnique.Errors;

        var address = LocationAddress.Create(
            command.Address.City,
            command.Address.Street,
            command.Address.HouseNumber).Value!;

        var timeZone = IANATimeZone.Create(command.TimeZone).Value!;

        var result = await _locationRepository.CreateAsync(
            entity: Location.Create(
                id: Id<Location>.GenerateNew(),
                name: name,
                address: address,
                timeZone: timeZone,
                createdAt: DateTime.UtcNow).Value!,
            cancellationToken: cancellationToken);

        if (result.IsFailure)
            return result.Errors;

        return LocationDTO.FromDomainEntity(result.Value);
    }
}
