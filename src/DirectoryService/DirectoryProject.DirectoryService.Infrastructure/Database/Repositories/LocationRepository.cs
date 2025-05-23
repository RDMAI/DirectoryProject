﻿using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DirectoryProject.DirectoryService.Infrastructure.Database.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly ApplicationDBContext _context;

    public LocationRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<Result<Location>> CreateAsync(
        Location entity,
        CancellationToken cancellationToken = default)
    {
        _context.Locations.Add(entity);
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
