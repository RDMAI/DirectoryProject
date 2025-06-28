using Core.Abstractions;
using DirectoryProject.DirectoryService.Application.DTOs;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.CreateLocation;

public record CreateLocationCommand(
    string Name,
    AddressDTO Address,
    string TimeZone) : ICommand;
