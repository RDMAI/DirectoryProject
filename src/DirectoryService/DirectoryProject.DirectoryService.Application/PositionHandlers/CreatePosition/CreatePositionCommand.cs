using Core.Abstractions;

namespace DirectoryProject.DirectoryService.Application.PositionHandlers.CreatePosition;

public record CreatePositionCommand(
    string Name,
    string Description,
    IEnumerable<Guid> DepartmentIds) : ICommand;
