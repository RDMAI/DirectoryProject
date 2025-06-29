using Core.Abstractions;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocationById;

public record GetLocationByIdQuery(
    Guid Id) : IQuery;
