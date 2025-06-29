using Core.Abstractions;
using DirectoryProject.DirectoryService.Application.DTOs;
using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
using SharedKernel;
using SharedKernel.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.PositionHandlers.UpdatePosition;

public class UpdatePositionHandler
    : ICommandHandler<UpdatePositionCommand, PositionDTO>
{
    private readonly AbstractValidator<UpdatePositionCommand> _validator;
    private readonly ILogger<UpdatePositionHandler> _logger;
    private readonly IPositionRepository _positionRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public UpdatePositionHandler(
        AbstractValidator<UpdatePositionCommand> validator,
        ILogger<UpdatePositionHandler> logger,
        IPositionRepository positionRepository,
        IDepartmentRepository departmentRepository)
    {
        _validator = validator;
        _logger = logger;
        _positionRepository = positionRepository;
        _departmentRepository = departmentRepository;
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
        var isNameChanged = entity.Name != name;
        if (isNameChanged)
        {
            var isNameUnique = await _positionRepository.IsNameUniqueAsync(name, cancellationToken);
            if (isNameUnique.IsFailure)
                return isNameUnique.Errors;
        }

        var description = PositionDescription.Create(command.Description).Value;
        var isDescriptionChanged = entity.Description != description;

        // validate positions
        var areDepartmentsChanged = AreDepartmentsChanged(entity, command);
        if (areDepartmentsChanged)
        {
            var departmentIds = command.DepartmentIds.Select(Id<Department>.Create);
            var areDepartmentsValidResult = await _departmentRepository.AreDepartmentsValidAsync(
                departmentIds,
                cancellationToken);
            if (areDepartmentsValidResult.IsFailure)
                return areDepartmentsValidResult.Errors;
        }

        if ((isNameChanged || isDescriptionChanged || areDepartmentsChanged) == false)
        {
            _logger.LogInformation(
                "Position with id {id} was unchanged, because request didn't have any changes",
                entity.Id);
            return PositionDTO.FromDomainEntity(entity);
        }

        if (isNameChanged)
        {
            entity.Update(
                name: name,
                description: entity.Description);
        }

        if (isDescriptionChanged)
        {
            entity.Update(
                name: entity.Name,
                description: description);
        }

        var oldDepartments = entity.DepartmentPositions;
        if (areDepartmentsChanged)
            entity.UpdateDepartments(command.DepartmentIds.Select(Id<Department>.Create));

        var updateResult = await _positionRepository.UpdateAsync(
            entity,
            cancellationToken,
            oldDepartments);
        if (updateResult.IsFailure)
            return updateResult.Errors;

        _logger.LogInformation("Position with id {id} was updated", entity.Id);

        return PositionDTO.FromDomainEntity(entity);
    }

    private bool AreDepartmentsChanged(
        Position entity,
        UpdatePositionCommand command)
    {
        if (entity.DepartmentPositions.Count != command.DepartmentIds.Count())
            return true;

        for (int i = 0; i < entity.DepartmentPositions.Count; i++)
        {
            if (entity.DepartmentPositions[i].DepartmentId.Value != command.DepartmentIds.ElementAt(i))
                return true;
        }

        return false;
    }
}
