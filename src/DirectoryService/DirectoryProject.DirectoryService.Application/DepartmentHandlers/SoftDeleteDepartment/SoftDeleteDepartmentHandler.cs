using Core.Abstractions;
using Core.Database;
using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using SharedKernel;
using SharedKernel.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.SoftDeleteDepartment;

public class SoftDeleteDepartmentHandler
    : ICommandHandler<SoftDeleteDepartmentCommand>
{
    private readonly AbstractValidator<SoftDeleteDepartmentCommand> _validator;
    private readonly ILogger<SoftDeleteDepartmentHandler> _logger;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteDepartmentHandler(
        AbstractValidator<SoftDeleteDepartmentCommand> validator,
        ILogger<SoftDeleteDepartmentHandler> logger,
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

    public async Task<UnitResult> HandleAsync(
        SoftDeleteDepartmentCommand command,
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

        // check if entity has children
        var childrenResult = await _departmentRepository.GetChildrenByPathAsync(
            entity.Path,
            cancellationToken);
        if (childrenResult.IsFailure) return childrenResult.Errors;
        if (childrenResult.Value.Any())
        {
            return ErrorHelper.Database.DeleteFailedConflict(
                $"Failed to delete Department with path {entity.Path}, because it has active children");
        }

        // check if entity has active locations
        //var locationsResult = await _locationRepository.GetLocationsForDepartmentAsync(
        //    entity.Id,
        //    cancellationToken);
        //if (locationsResult.IsFailure) return locationsResult.Errors;
        //if (locationsResult.Value.Any())
        //{
        //    return ErrorHelper.Database.DeleteFailedConflict(
        //        $"Failed to delete Department with id {entity.Id.Value}. It still has active locations");
        //}

        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        if (entity.Parent is not null)
        {
            entity.Parent.DecreaseChildrenCount();
            var parentUpdateResult = await _departmentRepository.UpdateAsync(
                entity.Parent,
                cancellationToken);
            if (parentUpdateResult.IsFailure)
                return parentUpdateResult.Errors;
        }

        entity.Deactivate();
        var entityUpdateResult = await _departmentRepository.UpdateAsync(
                entity,
                cancellationToken);
        if (entityUpdateResult.IsFailure)
        {
            transaction.Rollback();
            return entityUpdateResult.Errors;
        }

        transaction.Commit();

        _logger.LogInformation("Department with id {id} was deactivated", entity.Id);
        if (entity.Parent is not null)
            _logger.LogInformation("Parent with id {id} was updated", entity.Parent.Id);

        return UnitResult.Success();
    }
}
