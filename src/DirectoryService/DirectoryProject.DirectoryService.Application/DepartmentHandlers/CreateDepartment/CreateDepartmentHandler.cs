using Core.Abstractions;
using Core.Database;
using DirectoryProject.DirectoryService.Application.DTOs;
using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
using SharedKernel;
using SharedKernel.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.CreateDepartment;

public class CreateDepartmentHandler
    : ICommandHandler<CreateDepartmentCommand, DepartmentDTO>
{
    private readonly AbstractValidator<CreateDepartmentCommand> _validator;
    private readonly ILogger<CreateDepartmentHandler> _logger;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentHandler(
        AbstractValidator<CreateDepartmentCommand> validator,
        ILogger<CreateDepartmentHandler> logger,
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
        CreateDepartmentCommand command,
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

        // validate locations
        var locationIds = command.LocationIds.Select(Id<Location>.Create);
        var areLocationsValidResult = await _locationRepository.AreLocationsValidAsync(
            locationIds,
            cancellationToken);
        if (areLocationsValidResult.IsFailure)
            return areLocationsValidResult.Errors;

        Id<Department>? parentId = null;
        Department? parent = null;
        if (command.ParentId is not null)
        {
            // validate parent
            parentId = Id<Department>.Create(command.ParentId.Value);
            var parentResult = await _departmentRepository.GetByIdAsync(
                id: parentId,
                cancellationToken: cancellationToken);
            if (parentResult.IsFailure)
                return parentResult.Errors;

            parent = parentResult.Value;
        }

        var name = DepartmentName.Create(command.Name).Value;

        var pathResult = Department.CreatePath(name, parent?.Path);
        if (pathResult.IsFailure)
            return pathResult.Errors;

        var isPathUnique = await _departmentRepository.IsPathUniqueAsync(
            pathResult.Value,
            cancellationToken);
        if (isPathUnique.IsFailure)
            return isPathUnique.Errors;

        var entityResult = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: name,
            parent: parent,
            createdAt: DateTime.UtcNow);
        if (entityResult.IsFailure)
            return entityResult.Errors;

        var entity = entityResult.Value;

        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        entity.UpdateLocations(locationIds);

        var createResult = await _departmentRepository.CreateAsync(entity, cancellationToken);
        if (createResult.IsFailure)
        {
            transaction.Rollback();
            return createResult.Errors;
        }

        if (parent is not null)
        {
            parent.IncreaseChildrenCount();
            var updateParentResult = await _departmentRepository.UpdateAsync(parent, cancellationToken);
            if (updateParentResult.IsFailure)
            {
                transaction.Rollback();
                return updateParentResult.Errors;
            }
        }

        transaction.Commit();

        _logger.LogInformation(
            "Department created with id {0} name {1}",
            entity.Name.Value,
            entity.Id.Value);

        return DepartmentDTO.FromDomainEntity(entity);
    }
}
