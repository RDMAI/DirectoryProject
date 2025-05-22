﻿using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryProject.DirectoryService.Domain;

public class Department
{
    public Id<Department> Id { get; }
    public DepartmentName Name { get; }
    public Id<Department>? ParentId { get; }
    public Department? Parent { get; }
    public LTree Path { get; }
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
        string path,
        short depth,
        int childrenCount,
        DateTime createdAt)
    {
        if (string.IsNullOrEmpty(path))
            return ErrorHelper.General.ValueIsInvalid(nameof(Path));

        if (childrenCount < 0)
            return ErrorHelper.General.ValueIsInvalid(nameof(ChildrenCount));

        if (depth < 0)
            return ErrorHelper.General.ValueIsInvalid(nameof(Depth));

        return new Department(
            id,
            name,
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
        string path,
        short depth,
        int childrenCount,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Path = path;
        Depth = depth;
        ChildrenCount = childrenCount;
        CreatedAt = createdAt;
        UpdatedAt = CreatedAt;
    }
}
