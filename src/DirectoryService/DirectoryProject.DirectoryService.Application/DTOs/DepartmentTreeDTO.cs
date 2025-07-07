namespace DirectoryProject.DirectoryService.Application.DTOs;

public class DepartmentTreeDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid? ParentId { get; init; }
    public string Path { get; init; } = string.Empty;
    public short Depth { get; init; }
    public int ChildrenCount { get; init; }
    public string LogoUrl { get; set; } = string.Empty;

    public List<DepartmentDTO> Children { get; set; } = [];
}
