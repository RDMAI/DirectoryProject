using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.LocationHandlers.SoftDeleteLocation;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
