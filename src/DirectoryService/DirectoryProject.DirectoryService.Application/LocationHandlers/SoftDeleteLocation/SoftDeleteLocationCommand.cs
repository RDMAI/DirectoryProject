using DirectoryProject.DirectoryService.Application.Shared.Interfaces;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.SoftDeleteLocation;

public record SoftDeleteLocationCommand(
    Guid Id) : ICommand;
