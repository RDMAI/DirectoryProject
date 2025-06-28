using Core.Abstractions;
using DirectoryProject.DirectoryService.Application.DTOs;
using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using SharedKernel;
using SharedKernel.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.UpdateLocation;

public class UpdateLocationHandler
    : ICommandHandler<UpdateLocationCommand, LocationDTO>
{
    private readonly AbstractValidator<UpdateLocationCommand> _validator;
    private readonly ILogger<UpdateLocationHandler> _logger;
    private readonly ILocationRepository _locationRepository;

    public UpdateLocationHandler(
        AbstractValidator<UpdateLocationCommand> validator,
        ILogger<UpdateLocationHandler> logger,
        ILocationRepository locationRepository)
    {
        _validator = validator;
        _logger = logger;
        _locationRepository = locationRepository;
    }

    public async Task<Result<LocationDTO>> HandleAsync(
        UpdateLocationCommand command,
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

        // check if location exists
        var id = Id<Location>.Create(command.Id);
        var entityResult = await _locationRepository.GetByIdAsync(
            id,
            cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Errors;

        var entity = entityResult.Value;

        // check if new name is unique
        var name = LocationName.Create(command.Name).Value;
        if (entity.Name != name)
        {
            var isNameUnique = await _locationRepository.IsNameUniqueAsync(name, cancellationToken);
            if (isNameUnique.IsFailure)
                return isNameUnique.Errors;
        }

        entity.Update(
            name: name,
            address: LocationAddress.Create(
                city: command.Address.City,
                street: command.Address.Street,
                houseNumber: command.Address.HouseNumber).Value,
            timeZone: IANATimeZone.Create(command.TimeZone).Value);

        var updateResult = await _locationRepository.UpdateAsync(entity, cancellationToken);
        if (updateResult.IsFailure)
            return updateResult.Errors;

        _logger.LogInformation("Location with id {id} was updated", entity.Id);

        return LocationDTO.FromDomainEntity(entity);
    }
}
