using DirectoryProject.DirectoryService.Domain;

namespace DirectoryProject.DirectoryService.Application.DTOs;

public class DepartmentDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid? ParentId { get; init; }
    public string Path { get; init; } = string.Empty;
    public short Depth { get; init; }
    public int ChildrenCount { get; init; }
    public string LogoUrl { get; set; } = string.Empty;

    public static DepartmentDTO FromDomainEntity(Department entity, string? logoUrl = null)
        => new DepartmentDTO
        {
            Id = entity.Id.Value,
            Name = entity.Name.Value,
            ParentId = entity.ParentId?.Value,
            Path = entity.Path,
            Depth = entity.Depth,
            ChildrenCount = entity.ChildrenCount,
            LogoUrl = logoUrl ?? string.Empty,
        };
}
