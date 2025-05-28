using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryProject.DirectoryService.Infrastructure.Database.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDBContext _context;

    public DepartmentRepository(ApplicationDBContext context)
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

    public async Task<UnitResult> UpdateChildrenPathAsync(
        LTree oldPath,
        LTree newPath,
        CancellationToken cancellationToken = default)
    {
        // Intention:
        // UPDATE item
        // SET path = NEW.path || subpath(path, nlevel(OLD.path))
        // WHERE path<@ OLD.path;
        await _context.Departments
            .Where(d => d.IsActive)
            .Where(d => d.Path.IsDescendantOf(oldPath))
            .ExecuteUpdateAsync(
                propCall => propCall.SetProperty(
                d => d.Path,
                d => (LTree)(newPath + d.Path.Subpath(d.Path.NLevel))),
                cancellationToken);

        return UnitResult.Success();
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
            .Where(d => d.Id == id)
            .Select(d => new {
                Department = d,
                Parent = d.Parent,
            })
            .FirstOrDefaultAsync(cancellationToken);

        throw new NotImplementedException();
        //var entity = await _context.Departments
        //    .Include(d => d.Parent).FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        //if (entity is null)
        //    return ErrorHelper.General.NotFound(id.Value);

        //return entity;
    }

    public async Task<Result<Department>> UpdateAsync(
        Department entity,
        CancellationToken cancellationToken = default)
    {
        // Not needed because entity is tracked by EF Core,
        // and it wil auto update it when SaveChanges is called
        // var entry = _context.Departments.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }
}
