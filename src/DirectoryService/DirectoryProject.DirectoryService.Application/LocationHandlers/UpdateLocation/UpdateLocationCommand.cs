using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.UpdateLocation;

public record UpdateLocationCommand(
    Guid Id,
    string Name,
    AddressDTO Address,
    string TimeZone) : ICommand;
