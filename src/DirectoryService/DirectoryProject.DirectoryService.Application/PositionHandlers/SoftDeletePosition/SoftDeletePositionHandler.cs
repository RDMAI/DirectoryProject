using Core.Abstractions;
using Core.Database;
using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using SharedKernel;
using SharedKernel.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.PositionHandlers.SoftDeletePosition;

public class SoftDeletePositionHandler
    : ICommandHandler<SoftDeletePositionCommand>
{
    private readonly AbstractValidator<SoftDeletePositionCommand> _validator;
    private readonly ILogger<SoftDeletePositionHandler> _logger;
    private readonly IPositionRepository _positionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeletePositionHandler(
        AbstractValidator<SoftDeletePositionCommand> validator,
        ILogger<SoftDeletePositionHandler> logger,
        IPositionRepository positionRepository,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _logger = logger;
        _positionRepository = positionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UnitResult> HandleAsync(
        SoftDeletePositionCommand command,
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

        // check if entity exists
        var entityResult = await _positionRepository.GetByIdAsync(
            id: Id<Position>.Create(command.Id),
            cancellationToken: cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Errors;

        var entity = entityResult.Value;

        entity.Deactivate();

        var updateResult = await _positionRepository.UpdateAsync(entity, cancellationToken);
        if (updateResult.IsFailure)
            return updateResult.Errors;

        _logger.LogInformation("Position with id {id} was deactivated", entity.Id);

        return UnitResult.Success();
    }
}
