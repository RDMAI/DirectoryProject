using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
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
        DepartmentPath path,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Departments.FirstOrDefaultAsync(d => d.Path == path);
        if (entity is not null)
            return ErrorHelper.General.AlreadyExist(path.Value);

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
        var entity = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
        if (entity is null)
            return ErrorHelper.General.NotFound(id.Value);

        return entity;
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
