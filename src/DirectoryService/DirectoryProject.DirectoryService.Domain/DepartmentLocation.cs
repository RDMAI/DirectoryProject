using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;

namespace DirectoryProject.DirectoryService.Domain;

public class DepartmentLocation
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
}
