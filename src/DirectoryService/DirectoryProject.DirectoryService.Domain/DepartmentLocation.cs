using SharedKernel.ValueObjects;

namespace DirectoryProject.DirectoryService.Domain;

public class DepartmentLocation : IComparable<DepartmentLocation>, IEquatable<DepartmentLocation>
{
    public Id<Department> DepartmentId { get; }
    public Department Department { get; }
    public Id<Location> LocationId { get; }
    public Location Location { get; }

    public DepartmentLocation(
        Id<Department> departmentId,
        Id<Location> locationId)
    {
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    // EF Core
    private DepartmentLocation() {}

    public int CompareTo(DepartmentLocation? other)
    {
        if (other == null)
            return -1;

        if (DepartmentId.CompareTo(other.DepartmentId) == 0 && LocationId.CompareTo(other.LocationId) == 0)
            return 0;

        return -1;
    }

    public bool Equals(DepartmentLocation? other)
    {
        if (other is null)
            return false;
        return DepartmentId.Equals(other.DepartmentId) && LocationId.Equals(other.LocationId);
    }

    public override bool Equals(object? obj)
    {
        if (obj is DepartmentLocation other)
            return Equals(other);
        return false;
    }
}
