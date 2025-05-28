using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;

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

    Task<Result<IEnumerable<Department>>> GetFlatTreeAsync(
        LTree path,
        CancellationToken cancellationToken = default);

    Task<UnitResult> UpdateChildrenPathAsync(
        LTree oldPath,
        LTree newPath,
        CancellationToken cancellationToken = default);

    Task<UnitResult> IsPathUniqueAsync(
        LTree path,
        CancellationToken cancellationToken = default);
}
