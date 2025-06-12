using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationWriteDBContext _context;

    public DepartmentRepository(ApplicationWriteDBContext context)
    {
        _context = context;
    }

    public async Task<UnitResult> IsPathUniqueAsync(
        LTree path,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Departments
            .Where(d => d.IsActive)
            .FirstOrDefaultAsync(d => d.Path == path);
        if (entity is not null)
            return ErrorHelper.General.AlreadyExist(path);

        return UnitResult.Success();
    }

    public async Task<Result<IEnumerable<Department>>> GetFlatTreeAsync(
        LTree path,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.Departments
            .Where(d => d.IsActive)
            .Where(d => d.Path.IsAncestorOf(path)) // also returns itself
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<Result<IEnumerable<Department>>> GetChildrenByPathAsync(
        LTree path,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.Departments
            .Where(d => d.IsActive)
            .Where(d => d.Path.IsDescendantOf(path))
            .Where(d => d.Path != path)
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<Result<int>> UpdateChildrenPathAsync(
        LTree oldPath,
        LTree newPath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var oldDepth = Department.CalculateDepth(oldPath);
            var newDepth = Department.CalculateDepth(newPath);

            // Intention:
            // UPDATE item
            // SET path = NEW.path || subpath(path, nlevel(OLD.path))
            // WHERE path<@ OLD.path;
            var changedCount = await _context.Departments
                .Where(d => d.IsActive)
                .Where(d => d.Path.IsDescendantOf(oldPath))
                .Where(d => d.Path != newPath)
                .ExecuteUpdateAsync(
                    propCall => propCall
                    .SetProperty(
                        d => d.Path,
                        d => (LTree)(newPath + "." + d.Path.Subpath(oldDepth + 1)))
                    .SetProperty(
                        d => d.Depth,
                        d => d.Path.NLevel - 1 - oldDepth + newDepth),  // d.Path.NLevel - 1 won't work, because it will not be updated yet
                    cancellationToken);

            return changedCount;
        }
        catch (DbUpdateConcurrencyException)
        {
            return ErrorHelper.Tree.ConcurrentUpdateFailed(oldPath);
        }
    }

    public async Task<Result<Department>> CreateAsync(
        Department entity,
        CancellationToken cancellationToken = default)
    {
        _context.Departments.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<Result<Department>> GetByIdAsync(
        Id<Department> id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Departments
            .Include(d => d.Parent)
            .Include(d => d.DepartmentLocations)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (entity is null)
            return ErrorHelper.General.NotFound(id.Value);

        return entity;
    }

    public async Task<Result<Department>> UpdateAsync(
        Department entity,
        CancellationToken cancellationToken = default,
        IEnumerable<DepartmentLocation>? oldDepartmentLocations = null)
    {
        try
        {
            // sync DepartmentLocations
            if (oldDepartmentLocations is not null)
            {
                foreach (var item in oldDepartmentLocations)
                {
                    var found = entity.DepartmentLocations
                        .FirstOrDefault(d => d.LocationId == item.LocationId);
                    if (found is null)
                        _context.DepartmentLocations.Remove(item);
                }

                // this part was used to add new DepartmentLocations
                // after refactoring it is not needed anymore
                //foreach (var newItem in entity.DepartmentLocations)
                //{
                //    var found = oldDepartmentLocations
                //        .FirstOrDefault(d => d.LocationId == newItem.LocationId);
                //    if (found is null)
                //        _context.DepartmentLocations.Add(newItem);
                //}
            }

            // Not needed because entity is tracked by EF Core,
            // and it wil auto update it when SaveChanges is called
            // var entry = _context.Departments.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (DbUpdateConcurrencyException)
        {
            return ErrorHelper.Tree.ConcurrentUpdateFailed(entity.Id.Value);
        }
    }

    public async Task<Result<IEnumerable<Department>>> GetDepartmentsForLocationAsync(
        Id<Location> id,
        CancellationToken cancellationToken = default)
    {
        var result = await (
            from d in _context.Departments
            join dl in _context.DepartmentLocations on d.Id equals dl.DepartmentId
            where dl.LocationId == id
            select d).ToListAsync(cancellationToken);

        return result;
    }

    public async Task<UnitResult> AreDepartmentsValidAsync(
        IEnumerable<Id<Department>> departmentIds,
        CancellationToken cancellationToken = default)
    {
        var existingIds = await _context.Departments
            .Where(d => departmentIds.Contains(d.Id))
            .Select(d => d.Id)
            .ToListAsync(cancellationToken);

        foreach (var id in existingIds)
        {
            if (departmentIds.FirstOrDefault(id) is null)
                return ErrorHelper.General.NotFound(id.Value);
        }

        return UnitResult.Success();
    }
}
