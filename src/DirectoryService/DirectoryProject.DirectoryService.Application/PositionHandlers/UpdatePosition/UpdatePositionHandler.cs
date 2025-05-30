using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.PositionHandlers.UpdatePosition;

public class UpdatePositionHandler
    : ICommandHandler<UpdatePositionCommand, PositionDTO>
{
    private readonly AbstractValidator<UpdatePositionCommand> _validator;
    private readonly ILogger<UpdatePositionHandler> _logger;
    private readonly IPositionRepository _positionRepository;

    public UpdatePositionHandler(
        AbstractValidator<UpdatePositionCommand> validator,
        ILogger<UpdatePositionHandler> logger,
        IPositionRepository positionRepository)
    {
        _validator = validator;
        _logger = logger;
        _positionRepository = positionRepository;
    }

    public async Task<Result<PositionDTO>> HandleAsync(
        UpdatePositionCommand command,
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

        // check if position exists
        var id = Id<Position>.Create(command.Id);
        var entityResult = await _positionRepository.GetByIdAsync(
            id,
            cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Errors;

        var entity = entityResult.Value;

        // check if new name is unique
        var name = PositionName.Create(command.Name).Value;
        if (entity.Name != name)
        {
            var isNameUnique = await _positionRepository.IsNameUniqueAsync(name, cancellationToken);
            if (isNameUnique.IsFailure)
                return isNameUnique.Errors;
        }

        entity.Update(
            name: name,
            description: PositionDescription.Create(command.Description).Value);

        var updateResult = await _positionRepository.UpdateAsync(entity, cancellationToken);
        if (updateResult.IsFailure)
            return updateResult.Errors;

        return PositionDTO.FromDomainEntity(entity);
    }
}
