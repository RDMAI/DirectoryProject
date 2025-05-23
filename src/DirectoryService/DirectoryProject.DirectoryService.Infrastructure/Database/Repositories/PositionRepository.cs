using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace DirectoryProject.DirectoryService.Infrastructure.Database.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly ApplicationDBContext _context;

    public PositionRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<Result<Position>> CreateAsync(
        Position entity,
        CancellationToken cancellationToken = default)
    {
        _context.Positions.Add(entity);
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
