namespace DirectoryProject.DirectoryService.Application.DTOs;

public class DepartmentTreeDTO
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid? ParentId { get; private set; }
    public string Path { get; private set; }
    public short Depth { get; private set; }
    public int ChildrenCount { get; private set; }

    public List<DepartmentDTO> Children { get; set; } = [];

    public DepartmentTreeDTO(
        Guid id,
        string name,
        Guid? parentId,
        string path,
        short depth,
        int childrenCount)
    {
        Id = id;
        Name = name;
        ParentId = parentId;
        Path = path;
        Depth = depth;
        ChildrenCount = childrenCount;
    }
}
