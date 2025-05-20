using DirectoryProject.DirectoryService.Application.Shared.DTOs;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.CreateLocation;

public record CreateLocationCommand(
    string Name,
    AddressDTO Address,
    string TimeZone);
