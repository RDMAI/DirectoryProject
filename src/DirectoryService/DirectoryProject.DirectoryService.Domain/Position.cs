﻿using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;

namespace DirectoryProject.DirectoryService.Domain;

public class Position
{
    public Id<Position> Id { get; }
    public PositionName Name { get; private set; }
    public PositionDescription Description { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    private List<DepartmentPosition> _departmentPositions = [];
    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions.AsReadOnly();

    public static Result<Position> Create(
        Id<Position> id,
        PositionName name,
        PositionDescription description,
        DateTime createdAt)
    {
        return new Position(
            id,
            name,
            description,
            createdAt);
    }

    public Position Update(
        PositionName name,
        PositionDescription description)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;

        return this;
    }

    public Position UpdateDepartments(
        IEnumerable<Id<Department>> departmentIds)
    {
        _departmentPositions = departmentIds.Select(did => new DepartmentPosition(did, Id)).ToList();

        return this;
    }

    public Position Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public Position Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        return this;
    }

    // EF Core
    private Position() { }

    private Position(
        Id<Position> id,
        PositionName name,
        PositionDescription description,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = CreatedAt;
    }
}
