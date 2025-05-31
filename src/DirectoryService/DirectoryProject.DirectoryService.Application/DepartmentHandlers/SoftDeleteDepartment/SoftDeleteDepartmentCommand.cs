using DirectoryProject.DirectoryService.Application.Shared.Interfaces;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.SoftDeleteDepartment;

public record SoftDeleteDepartmentCommand(
    Guid Id) : ICommand;
