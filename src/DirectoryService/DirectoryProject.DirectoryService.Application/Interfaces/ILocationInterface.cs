using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;

namespace DirectoryProject.DirectoryService.Application.Interfaces;

public interface ILocationRepository
{
    Task<Result<Location>> CreateAsync(
        Location entity,
        CancellationToken cancellationToken = default);

    Task<UnitResult> IsNameUniqueAsync(
        LocationName name,
        CancellationToken cancellationToken = default);
}
