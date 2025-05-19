using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;

namespace DirectoryProject.DirectoryService.Domain;

public class Location
{
    public Id<Location> Id { get; }
    public LocationName Name { get; }
    public LocationAddress Address { get; }
    public IANATimeZone TimeZone { get; }
    public bool IsActive { get; } = true;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

    public List<DepartmentLocation> DepartmentLocations { get; } = [];

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
