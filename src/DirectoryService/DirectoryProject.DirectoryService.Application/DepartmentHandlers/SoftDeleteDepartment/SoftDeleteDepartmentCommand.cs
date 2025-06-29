using Core.Abstractions;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.SoftDeleteDepartment;

public record SoftDeleteDepartmentCommand(
    Guid Id) : ICommand;
