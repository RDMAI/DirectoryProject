using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly ApplicationWriteDBContext _context;

    public PositionRepository(ApplicationWriteDBContext context)
    {
        _context = context;
    }

    public async Task<Result<Position>> GetByIdAsync(
        Id<Position> id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Positions.FirstOrDefaultAsync(d => d.Id == id);
        if (entity is null)
            return ErrorHelper.General.NotFound(id.Value);

        return entity;
    }

    public async Task<Result<Position>> GetByNameAsync(
        PositionName name,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Positions.FirstOrDefaultAsync(d => d.Name == name);
        if (entity is null)
            return ErrorHelper.General.NotFound(name.Value);

        return entity;
    }

    public async Task<Result<Position>> CreateAsync(
        Position entity,
        CancellationToken cancellationToken = default)
    {
        _context.Positions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<Result<Position>> UpdateAsync(
        Position entity,
        CancellationToken cancellationToken = default,
        IEnumerable<DepartmentPosition>? oldDepartmentPositions = null)
    {
        // sync DepartmentPositions
        if (oldDepartmentPositions is not null)
        {
            foreach (var item in oldDepartmentPositions)
            {
                var found = entity.DepartmentPositions
                    .FirstOrDefault(d => d.PositionId == item.PositionId);
                if (found is null)
                    _context.DepartmentPositions.Remove(item);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<UnitResult> IsNameUniqueAsync(
        PositionName name,
        CancellationToken cancellationToken = default)
    {
        var existingPosition = await _context.Positions.FirstOrDefaultAsync(d => d.Name == name);

        if (existingPosition is not null)
            return ErrorHelper.General.AlreadyExist(name.Value);

        return UnitResult.Success();
    }
}
