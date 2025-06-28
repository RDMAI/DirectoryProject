using DirectoryProject.DirectoryService.Domain;
using SharedKernel;
using SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryProject.DirectoryService.Application.Interfaces;

public interface IDepartmentRepository
{
    Task<Result<Department>> CreateAsync(
        Department entity,
        CancellationToken cancellationToken = default);

    Task<Result<Department>> UpdateAsync(
        Department entity,
        CancellationToken cancellationToken = default,
        IEnumerable<DepartmentLocation>? oldDepartmentLocations = null);

    Task<Result<Department>> GetByIdAsync(
        Id<Department> id,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Department>>> GetDepartmentsForLocationAsync(
        Id<Location> id,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Department>>> GetFlatTreeAsync(
        LTree path,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Department>>> GetChildrenByPathAsync(
        LTree path,
        CancellationToken cancellationToken = default);

    Task<Result<int>> UpdateChildrenPathAsync(
        LTree oldPath,
        LTree newPath,
        CancellationToken cancellationToken = default);

    Task<UnitResult> IsPathUniqueAsync(
        LTree path,
        CancellationToken cancellationToken = default);

    Task<UnitResult> AreDepartmentsValidAsync(
        IEnumerable<Id<Department>> departmentIds,
        CancellationToken cancellationToken = default);
}
