﻿using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.PositionHandlers.CreatePosition;

public class CreatePositionHandler
    : ICommandHandler<CreatePositionCommand, PositionDTO>
{
    private readonly AbstractValidator<CreatePositionCommand> _validator;
    private readonly ILogger<CreatePositionHandler> _logger;
    private readonly IPositionRepository _positionRepository;

    public CreatePositionHandler(
        AbstractValidator<CreatePositionCommand> validator,
        IPositionRepository positionRepository,
        ILogger<CreatePositionHandler> logger)
    {
        _validator = validator;
        _positionRepository = positionRepository;
        _logger = logger;
    }

    public async Task<Result<PositionDTO>> HandleAsync(
        CreatePositionCommand command,
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

        var name = PositionName.Create(command.Name).Value!;
        var isNameUnique = await _positionRepository.IsNameUniqueAsync(name, cancellationToken);
        if (isNameUnique.IsFailure)
            return isNameUnique.Errors;

        var result = await _positionRepository.CreateAsync(
            entity: Position.Create(
                id: Id<Position>.GenerateNew(),
                name: name,
                description: PositionDescription.Create(command.Description).Value,
                createdAt: DateTime.UtcNow).Value!,
            cancellationToken: cancellationToken);

        if (result.IsFailure)
            return result.Errors;

        _logger.LogInformation(
            "Position created with id {0} name {1}",
            result.Value.Name.Value,
            result.Value.Id.Value);

        return PositionDTO.FromDomainEntity(result.Value);
    }
}
