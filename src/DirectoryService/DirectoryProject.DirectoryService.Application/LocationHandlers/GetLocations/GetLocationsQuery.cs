using Core.Abstractions;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocations;

public record GetLocationsQuery(
    int Page,
    int Size,
    string? Search = null) : IQuery;
