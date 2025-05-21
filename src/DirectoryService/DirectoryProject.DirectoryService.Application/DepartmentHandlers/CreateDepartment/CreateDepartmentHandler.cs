using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.Services;
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
    private readonly SlugService _slugService;

    public CreateDepartmentHandler(
        AbstractValidator<CreateDepartmentCommand> validator,
        ILogger<CreateDepartmentHandler> logger,
        IDepartmentRepository departmentRepository,
        SlugService slugService)
    {
        _validator = validator;
        _logger = logger;
        _departmentRepository = departmentRepository;
        _slugService = slugService;
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
        var areLocationsValidResult = await _departmentRepository.AreLocationsValidAsync(
            locationIds,
            cancellationToken);
        if (areLocationsValidResult.IsFailure)
            return areLocationsValidResult.Errors;

        Id<Department>? parentId = null;
        if (command.ParentId is not null)
        {
            // validate parent
            parentId = Id<Department>.Create(command.ParentId.Value);
            var parentResult = await _departmentRepository.GetByIdAsync(
                parentId,
                cancellationToken);
            if (parentResult.IsFailure)
                return parentResult.Errors;

            var entityResult = Department.Create(
                id: Id<Department>.GenerateNew(),
                name: DepartmentName.Create(command.Name).Value,
                path: $"{parentResult.Value.Path}.{_slugService.ConvertStringToSlug(command.Name)}",
                depth: (short)(parentResult.Value.Depth + 1),
                childrenCount: 0,
                createdAt: DateTime.UtcNow);

            if (entityResult.IsFailure)
                return entityResult.Errors;

            
        }
        
        //Формирование Path

        //    slug = kebab‑case(name) → dev-team, finansovyj-otdel, …
        //    Path = parent.Path + '.' + slug или slug для корня.
        //    Depth = parent.Depth + 1 (или 0 для корня).
        //    ChildrenCount = 0.

        //Транзакция

        //    Вставить запись в Department.
        //    Вставить строки в DepartmentLocation для всех locationIds.
        //    Обновить ChildrenCount у родителя (если он есть).

        //Ответ — 201 Created + DTO созданного отдела.
    }
}
