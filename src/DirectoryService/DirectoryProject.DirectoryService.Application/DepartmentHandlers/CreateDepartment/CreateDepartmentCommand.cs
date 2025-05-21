using DirectoryProject.DirectoryService.Application.Shared.Interfaces;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.CreateDepartment;

public record CreateDepartmentCommand(
    string Name,
    IEnumerable<Guid> LocationIds,
    Guid? ParentId = null) : ICommand;
