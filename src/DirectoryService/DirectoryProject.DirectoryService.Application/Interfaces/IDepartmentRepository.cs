using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;

namespace DirectoryProject.DirectoryService.Application.Interfaces;

public interface IDepartmentRepository
{
    Task<Result<Department>> CreateAsync(
        Department entity,
        CancellationToken cancellationToken = default);

    Task<Result<Department>> UpdateAsync(
        Department entity,
        CancellationToken cancellationToken = default);

    Task<Result<Department>> GetByIdAsync(
        Id<Department> id,
        CancellationToken cancellationToken = default);

    Task<UnitResult> IsPathUniqueAsync(
        string path,
        CancellationToken cancellationToken = default);

    Task<UnitResult> AreLocationsValidAsync(
        IEnumerable<Id<Location>> locationIds,
        CancellationToken cancellationToken = default);
}
