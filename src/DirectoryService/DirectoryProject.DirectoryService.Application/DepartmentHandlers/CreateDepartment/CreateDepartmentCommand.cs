using Core.Abstractions;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.CreateDepartment;

public record CreateDepartmentCommand(
    string Name,
    IEnumerable<Guid> LocationIds,
    Guid? ParentId = null) : ICommand;
