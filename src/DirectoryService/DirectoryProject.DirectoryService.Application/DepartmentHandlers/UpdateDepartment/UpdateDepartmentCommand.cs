using DirectoryProject.DirectoryService.Application.Shared.Interfaces;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.UpdateDepartment;

public record UpdateDepartmentCommand(
    Guid Id,
    string Name,
    IEnumerable<Guid> LocationIds,
    Guid? ParentId = null) : ICommand;
