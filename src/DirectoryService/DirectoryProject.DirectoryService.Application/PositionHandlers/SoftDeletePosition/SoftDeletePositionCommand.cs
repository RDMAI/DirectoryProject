using Core.Abstractions;

namespace DirectoryProject.DirectoryService.Application.PositionHandlers.SoftDeletePosition;

public record SoftDeletePositionCommand(
    Guid Id) : ICommand;
