using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.CreateLocation;

public record CreateLocationCommand(
    string Name,
    AddressDTO Address,
    string TimeZone) : ICommand;
