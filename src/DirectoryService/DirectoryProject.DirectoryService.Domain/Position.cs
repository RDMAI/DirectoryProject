using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;

namespace DirectoryProject.DirectoryService.Domain;

public class Position
{
    public Id<Position> Id { get; }
    public PositionName Name { get; }
    public PositionDescription Description { get; }
    public bool IsActive { get; } = true;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

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

    public Position Update()
    {


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
