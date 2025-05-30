using System.Data;
using System.Text;
using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.UpdateDepartment;

public class UpdateDepartmentHandler
    : ICommandHandler<UpdateDepartmentCommand, DepartmentDTO>
{
    private readonly AbstractValidator<UpdateDepartmentCommand> _validator;
    private readonly ILogger<UpdateDepartmentHandler> _logger;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDepartmentHandler(
        AbstractValidator<UpdateDepartmentCommand> validator,
        ILogger<UpdateDepartmentHandler> logger,
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _logger = logger;
        _departmentRepository = departmentRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DepartmentDTO>> HandleAsync(
        UpdateDepartmentCommand command,
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
        var entityResult = await _departmentRepository.GetByIdAsync(
            id: Id<Department>.Create(command.Id),
            cancellationToken: cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Errors;

        var entity = entityResult.Value;

        var newName = DepartmentName.Create(command.Name).Value;
        bool isNameChanged = entity.Name.Value != newName.Value;

        var updatedParentId = command.ParentId is null ? null : Id<Department>.Create(command.ParentId!.Value);
        bool isParentChanged = entity.ParentId != updatedParentId;

        // Get new parent if not null
        Department? newParent = null;
        if (isParentChanged && updatedParentId is not null)
        {
            var parentResult = await _departmentRepository.GetByIdAsync(
                id: updatedParentId,
                cancellationToken: cancellationToken);
            if (parentResult.IsFailure)
                return parentResult.Errors;

            newParent = parentResult.Value;

            var isTreeWithoutCyclesResult = await IsTreeWithoutCyclesAsync(
                entity,
                newParent.Path,
                cancellationToken);
            if (isTreeWithoutCyclesResult.IsFailure)
                return isTreeWithoutCyclesResult.Errors;
        }

        // validate new Path
        Result<LTree>? updatedPathResult = Department.CreatePath(
            isNameChanged ? newName : entity.Name,
            isParentChanged ? newParent?.Path : entity.Parent?.Path);
        if (updatedPathResult.IsFailure)
            return updatedPathResult.Errors;

        // validate locations
        var areLocationsChanged = AreLocationChanged(entity, command);
        if (areLocationsChanged)
        {
            var locationIds = command.LocationIds.Select(Id<Location>.Create);
            var areLocationsValidResult = await _locationRepository.AreLocationsValidAsync(
                locationIds,
                cancellationToken);
            if (areLocationsValidResult.IsFailure)
                return areLocationsValidResult.Errors;
        }

        // ef entity is unchanged return
        if ((isNameChanged || isParentChanged || areLocationsChanged) == false)
        {
            _logger.LogInformation(
                "Department with id {id} was unchanged, because request didn't have any changes",
                entity.Id);
            return DepartmentDTO.FromDomainEntity(entity);
        }

        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var oldParent = entity.Parent;
        var oldPath = entity.Path;
        var oldLocations = entity.DepartmentLocations;

        // update entity
        if (isNameChanged || isParentChanged)
        {
            entity.Update(
                name: isNameChanged ? newName : entity.Name,
                parentId: isParentChanged ? newParent?.Id : entity.ParentId,
                path: updatedPathResult.Value);
        }

        if (areLocationsChanged)
            entity.UpdateLocations(command.LocationIds.Select(Id<Location>.Create));

        // update old parent if not null
        if (isParentChanged && oldParent is not null)
        {
            oldParent.DecreaseChildrenCount();
            var oldParentUpdateResult = await _departmentRepository.UpdateAsync(oldParent, cancellationToken);
            if (oldParentUpdateResult.IsFailure)
            {
                transaction.Rollback();
                return oldParentUpdateResult.Errors;
            }
        }

        // update new parent if not null
        if (isParentChanged && newParent is not null)
        {
            newParent.IncreaseChildrenCount();
            var newParentUpdateResult = await _departmentRepository.UpdateAsync(newParent, cancellationToken);
            if (newParentUpdateResult.IsFailure)
            {
                transaction.Rollback();
                return newParentUpdateResult.Errors;
            }
        }

        // update entity in database
        if (isNameChanged || isParentChanged || areLocationsChanged)
        {
            var entityUpdateResult = await _departmentRepository.UpdateAsync(
                entity,
                cancellationToken,
                oldLocations);
            if (entityUpdateResult.IsFailure)
            {
                transaction.Rollback();
                return entityUpdateResult.Errors;
            }
        }

        // update children if parent changed
        int childrenChangedCount = 0;
        if (isNameChanged || isParentChanged)
        {
            var childrenUpdateResult = await _departmentRepository.UpdateChildrenPathAsync(
                oldPath,
                entity.Path,
                cancellationToken);
            if (childrenUpdateResult.IsFailure)
            {
                transaction.Rollback();
                return childrenUpdateResult.Errors;
            }

            childrenChangedCount = childrenUpdateResult.Value;
        }

        transaction.Commit();

        _logger.LogInformation("Department with id {id} was updated", entity.Id);
        if (isParentChanged && oldParent is not null)
            _logger.LogInformation("Old parent with id {id} was updated", oldParent.Id);
        if (isParentChanged && newParent is not null)
            _logger.LogInformation("New parent with id {id} was updated", newParent.Id);
        if (childrenChangedCount > 0)
            _logger.LogInformation("Updated {count} children", childrenChangedCount);

        return DepartmentDTO.FromDomainEntity(entity);
    }

    private async Task<UnitResult> IsTreeWithoutCyclesAsync(
        Department entity,
        LTree parentPath,
        CancellationToken cancellationToken = default)
    {
        // check if tree already contains entity's id
        var parentFlatTreeResult = await _departmentRepository.GetFlatTreeAsync(parentPath, cancellationToken);
        if (parentFlatTreeResult.IsFailure)
            return parentFlatTreeResult.Errors;

        if (parentFlatTreeResult.Value.FirstOrDefault(d => d.Id == entity.Id) is not null)
            return ErrorHelper.Tree.CycleInTree(entity.Id.Value);

        return UnitResult.Success();
    }

    private bool AreLocationChanged(
        Department entity,
        UpdateDepartmentCommand command)
    {
        if (entity.DepartmentLocations.Count != command.LocationIds.Count())
            return true;

        for (int i = 0; i < entity.DepartmentLocations.Count; i++)
        {
            if (entity.DepartmentLocations[i].LocationId.Value != command.LocationIds.ElementAt(i))
                return true;
        }

        return false;
    }
}
