using DirectoryProject.DirectoryService.Application.Shared.Interfaces;

namespace DirectoryProject.DirectoryService.Application.PositionHandlers.SoftDeletePosition;

public record SoftDeletePositionCommand(
    Guid Id) : ICommand;
