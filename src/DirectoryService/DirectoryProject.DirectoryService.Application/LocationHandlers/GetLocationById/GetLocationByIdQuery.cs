using DirectoryProject.DirectoryService.Application.Shared.Interfaces;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocationById;

public record GetLocationByIdQuery(
    Guid Id) : IQuery;
