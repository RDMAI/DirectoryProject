using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using FluentValidation;
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

        // validate parent
        var newParentId = command.ParentId is null ? null : Id<Department>.Create(command.ParentId!.Value);
        Department? parent = entity.Parent;
        if (entity.ParentId != newParentId && newParentId is not null)
        {
            var parentResult = await _departmentRepository.GetByIdAsync(
                id: newParentId,
                loadFullBranch: true,
                cancellationToken: cancellationToken);
            if (parentResult.IsFailure)
                return parentResult.Errors;

            parent = parentResult.Value;
        }

        // check if tree already contains entity's id
        if (parent is not null)
        {
            var parentFlatTreeResult = await _departmentRepository.GetFlatTreeAsync(parent.Path, cancellationToken);
            if (parentFlatTreeResult.IsFailure)
                return parentFlatTreeResult.Errors;

            if (parentFlatTreeResult.Value.FirstOrDefault(d => d.Id == entity.Id) is not null)
                return ErrorHelper.Tree.CycleInTree(entity.Id.Value);
        }

        // validate locations
        var locationIds = command.LocationIds.Select(Id<Location>.Create);
        var areLocationsValidResult = await _locationRepository.AreLocationsValidAsync(
            locationIds,
            cancellationToken);
        if (areLocationsValidResult.IsFailure)
            return areLocationsValidResult.Errors;




        //var name = DepartmentName.Create(command.Name).Value;
        //var pathResult = DepartmentPath.CreateFromStringAndParent(name.Value, parent?.Path.Value);

        //var parentId = 
        //if ()

        //if (entity.Name != name)
        //{
        //    var newPath = DepartmentPath.CreateFromStringAndParent(name.Value, entity.Parent?.Name.Value);
        //}

        throw new NotImplementedException();
    }
}
