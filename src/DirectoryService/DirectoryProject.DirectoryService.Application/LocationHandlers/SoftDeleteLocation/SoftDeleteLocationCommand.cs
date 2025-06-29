using Core.Abstractions;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.SoftDeleteLocation;

public record SoftDeleteLocationCommand(
    Guid Id) : ICommand;
