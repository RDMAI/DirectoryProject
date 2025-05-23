using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;

namespace DirectoryProject.DirectoryService.Domain;

public class Department
{
    public Id<Department> Id { get; }
    public DepartmentName Name { get; }
    public Id<Department>? ParentId { get; }
    public Department? Parent { get; }
    public DepartmentPath Path { get; }
    public short Depth { get; }
    public int ChildrenCount { get; private set; }
    public bool IsActive { get; } = true;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

    private readonly List<DepartmentLocation> _departmentLocations = [];
    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations.ToList();

    public static Result<Department> Create(
        Id<Department> id,
        DepartmentName name,
        Id<Department>? parentId,
        DepartmentPath path,
        short depth,
        int childrenCount,
        DateTime createdAt)
    {
        if (childrenCount < 0)
            return ErrorHelper.General.ValueIsInvalid(nameof(ChildrenCount));

        if (depth < 0)
            return ErrorHelper.General.ValueIsInvalid(nameof(Depth));

        return new Department(
            id,
            name,
            parentId,
            path,
            depth,
            childrenCount,
            createdAt);
    }

    public Department IncreaseChildrenCount()
    {
        ChildrenCount++;
        return this;
    }

    public Department AddLocations(IEnumerable<Id<Location>> locationIds)
    {
        var locationDepartments = locationIds.Select(l =>
            new DepartmentLocation(
                departmentId: Id,
                locationId: l));
        _departmentLocations.AddRange(locationDepartments);
        return this;
    }

    // EF Core
    private Department() { }

    private Department(
        Id<Department> id,
        DepartmentName name,
        Id<Department>? parentId,
        DepartmentPath path,
        short depth,
        int childrenCount,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        ParentId = parentId;
        Path = path;
        Depth = depth;
        ChildrenCount = childrenCount;
        CreatedAt = createdAt;
        UpdatedAt = CreatedAt;
    }
}
