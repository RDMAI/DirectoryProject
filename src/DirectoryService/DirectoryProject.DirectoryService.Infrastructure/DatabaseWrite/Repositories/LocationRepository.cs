using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly ApplicationWriteDBContext _context;

    public LocationRepository(ApplicationWriteDBContext context)
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

    public async Task<Result<(int TotalCount, IEnumerable<Location> Values)>> GetAsync(
        Func<IQueryable<Location>, IQueryable<Location>> filterQuery,
        int Page,
        int PageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Location> set = _context.Locations;
        if (filterQuery is not null)
            set = filterQuery(set);

        var totalCount = await set.CountAsync(cancellationToken);

        var result = await set
            .Skip((Page - 1) * PageSize).Take(PageSize)
            .ToListAsync(cancellationToken);

        return (totalCount, result);
    }

    public async Task<Result<IEnumerable<Location>>> GetLocationsForDepartmentAsync(
        Id<Department> id,
        CancellationToken cancellationToken = default)
    {
        var result = await (
            from l in _context.Locations
            join dl in _context.DepartmentLocations on l.Id equals dl.LocationId
            where dl.DepartmentId == id
            select l).ToListAsync(cancellationToken);

        return result;
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

    public async Task<UnitResult> IsAddressUniqueAsync(
        LocationAddress address,
        CancellationToken cancellationToken = default)
    {
        var existingLocation = await _context.Locations
            .FirstOrDefaultAsync(d => d.Address.City == address.City &&
                d.Address.Street == address.Street &&
                d.Address.HouseNumber == address.HouseNumber);

        if (existingLocation is not null)
            return ErrorHelper.General.AlreadyExist($"{address.City} {address.Street} {address.HouseNumber}");

        return UnitResult.Success();
    }

    public async Task<UnitResult> AreLocationsValidAsync(
        IEnumerable<Id<Location>> locationIds,
        CancellationToken cancellationToken = default)
    {
        var existingIds = await _context.Locations
            .Where(l => locationIds.Contains(l.Id))
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);

        foreach (var id in existingIds)
        {
            if (locationIds.FirstOrDefault(id) is null)
                return ErrorHelper.General.NotFound(id.Value);
        }

        return UnitResult.Success();
    }
}
