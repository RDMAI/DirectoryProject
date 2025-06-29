using Core.Abstractions;
using DirectoryProject.DirectoryService.Application.DTOs;
using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using SharedKernel;
using SharedKernel.ValueObjects;
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
        var isAddressUnique = await _locationRepository.IsAddressUniqueAsync(address, cancellationToken);
        if (isAddressUnique.IsFailure)
            return isAddressUnique.Errors;

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

        _logger.LogInformation(
            "Location created with id {0} name {1}",
            result.Value.Name.Value,
            result.Value.Id.Value);

        return LocationDTO.FromDomainEntity(result.Value);
    }
}
