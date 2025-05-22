using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;

namespace DirectoryProject.DirectoryService.Domain;

public class Location
{
    public Id<Location> Id { get; }
    public LocationName Name { get; private set; }
    public LocationAddress Address { get; private set; }
    public IANATimeZone TimeZone { get; private set; }
    public bool IsActive { get; } = true;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<DepartmentLocation> _departmentLocations = [];
    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations.ToList();

    public static Result<Location> Create(
        Id<Location> id,
        LocationName name,
        LocationAddress address,
        IANATimeZone timeZone,
        DateTime createdAt)
    {
        return new Location(
            id,
            name,
            address,
            timeZone,
            createdAt);
    }

    public Location Update(
        LocationName name,
        LocationAddress address,
        IANATimeZone timeZone)
    {
        Name = name;
        Address = address;
        TimeZone = timeZone;
        UpdatedAt = DateTime.UtcNow;

        return this;
    }

    // EF Core
    private Location() { }

    private Location(
        Id<Location> id,
        LocationName name,
        LocationAddress address,
        IANATimeZone timeZone,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Address = address;
        TimeZone = timeZone;
        CreatedAt = createdAt;
        UpdatedAt = CreatedAt;
    }
}
