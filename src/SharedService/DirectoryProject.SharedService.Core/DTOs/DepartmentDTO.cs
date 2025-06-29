namespace DirectoryProject.SharedService.Core.DTOs;

public record DepartmentDTO(
    Guid Id,
    string Name,
    Guid? ParentId,
    string Path,
    short Depth,
    int ChildrenCount);
