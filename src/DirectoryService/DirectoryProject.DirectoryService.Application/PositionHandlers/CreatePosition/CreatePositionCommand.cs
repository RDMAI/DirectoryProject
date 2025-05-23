using DirectoryProject.DirectoryService.Application.Shared.Interfaces;

namespace DirectoryProject.DirectoryService.Application.PositionHandlers.CreatePosition;

public record CreatePositionCommand(
    string Name,
    string Description) : ICommand;
