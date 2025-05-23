﻿using System.Text;
using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
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
                parentId,
                cancellationToken);
            if (parentResult.IsFailure)
                return parentResult.Errors;

            parent = parentResult.Value;
        }

        var path = DepartmentPath.CreateFromStringAndParent(command.Name, parent?.Path.Value);

        var isPathUnique = await _departmentRepository.IsPathUniqueAsync(
            path,
            cancellationToken);
        if (isPathUnique.IsFailure)
            return isPathUnique.Errors;

        var entityResult = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create(command.Name).Value,
            parentId: parentId,
            path: path,
            depth: parent is null ?
                (short)0 :
                (short)(parent.Depth + 1),
            childrenCount: 0,
            createdAt: DateTime.UtcNow);

        if (entityResult.IsFailure)
            return entityResult.Errors;
        var entity = entityResult.Value;

        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        entity.AddLocations(locationIds);

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
