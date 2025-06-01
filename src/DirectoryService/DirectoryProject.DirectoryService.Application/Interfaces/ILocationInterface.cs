using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;

namespace DirectoryProject.DirectoryService.Application.Interfaces;

public interface ILocationRepository
{
    Task<Result<Location>> GetByIdAsync(
        Id<Location> id,
        CancellationToken cancellationToken = default);

    Task<Result<Location>> GetByNameAsync(
        LocationName name,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Location>>> GetAsync(
        Func<IQueryable<Location>, IQueryable<Location>> filterQuery,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Location>>> GetLocationsForDepartmentAsync(
        Id<Department> id,
        CancellationToken cancellationToken = default);

    Task<Result<Location>> CreateAsync(
        Location entity,
        CancellationToken cancellationToken = default);

    Task<Result<Location>> UpdateAsync(
        Location entity,
        CancellationToken cancellationToken = default);

    Task<UnitResult> IsNameUniqueAsync(
        LocationName name,
        CancellationToken cancellationToken = default);

    Task<UnitResult> AreLocationsValidAsync(
        IEnumerable<Id<Location>> locationIds,
        CancellationToken cancellationToken = default);
}
