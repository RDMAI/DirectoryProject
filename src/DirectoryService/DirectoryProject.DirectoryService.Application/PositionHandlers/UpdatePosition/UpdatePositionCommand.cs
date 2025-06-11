using DirectoryProject.DirectoryService.Application.Shared.Interfaces;

namespace DirectoryProject.DirectoryService.Application.PositionHandlers.UpdatePosition;

public record UpdatePositionCommand(
    Guid Id,
    string Name,
    string Description,
    IEnumerable<Guid> DepartmentIds) : ICommand;
