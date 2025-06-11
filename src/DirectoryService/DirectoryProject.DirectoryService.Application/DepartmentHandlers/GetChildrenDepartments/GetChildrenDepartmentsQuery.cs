using DirectoryProject.DirectoryService.Application.Shared.Interfaces;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetChildrenDepartments;

public record GetChildrenDepartmentsQuery(
    Guid ParentId,
    int Page,
    int Size) : IQuery;
