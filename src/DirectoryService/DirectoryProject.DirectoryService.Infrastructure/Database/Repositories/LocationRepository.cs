using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryProject.DirectoryService.Infrastructure.Database.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly ApplicationDBContext _context;

    public LocationRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<Result<Location>> GetByIdAsync(
        Id<Location> id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Locations.FirstOrDefaultAsync(d => d.Id == id);
        if (entity is null)
            return ErrorHelper.General.NotFound(id.Value);

        return entity;
    }

    public async Task<Result<Location>> GetByNameAsync(
        LocationName name,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Locations.FirstOrDefaultAsync(d => d.Name == name);
        if (entity is null)
            return ErrorHelper.General.NotFound(name.Value);

        return entity;
    }

    public async Task<Result<Location>> CreateAsync(
        Location entity,
        CancellationToken cancellationToken = default)
    {
        _context.Locations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<Result<Location>> UpdateAsync(
        Location entity,
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<UnitResult> IsNameUniqueAsync(
        LocationName name,
        CancellationToken cancellationToken = default)
    {
        var existingLocation = await _context.Locations.FirstOrDefaultAsync(d => d.Name == name);

        if (existingLocation is not null)
            return ErrorHelper.General.AlreadyExist(name.Value);

        return UnitResult.Success();
    }
}
