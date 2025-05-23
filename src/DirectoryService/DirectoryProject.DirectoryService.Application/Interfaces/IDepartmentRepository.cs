using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
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
        DepartmentPath path,
        CancellationToken cancellationToken = default);
}
