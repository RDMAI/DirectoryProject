using Core.Abstractions;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetRootDepartments;

public record GetRootDepartmentsQuery(
    int Page,
    int Size,
    int Prefetch) : IQuery;
