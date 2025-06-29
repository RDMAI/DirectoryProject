namespace DirectoryProject.SharedService.Core.DTOs;

public class DepartmentTreeDTO
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Path { get; private set; }
    public short Depth { get; private set; }
    public Guid? ParentId { get; private set; }
    public List<DepartmentTreeDTO> Children { get; private set; } = [];
    public int ChildrenCount { get; private set; }

    public DepartmentTreeDTO()
    {
    }
}
