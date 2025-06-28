using Core.Abstractions;
using DirectoryProject.DirectoryService.Application.DTOs;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.UpdateLocation;

public record UpdateLocationCommand(
    Guid Id,
    string Name,
    AddressDTO Address,
    string TimeZone) : ICommand;
