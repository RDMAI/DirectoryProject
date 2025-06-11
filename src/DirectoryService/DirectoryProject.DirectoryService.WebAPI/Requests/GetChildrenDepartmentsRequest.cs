using DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetChildrenDepartments;

namespace DirectoryProject.DirectoryService.WebAPI.Requests;

public record GetChildrenDepartmentsRequest(
    int Page,
    int Size)
{
    public GetChildrenDepartmentsQuery ToQuery(Guid parentId) =>
        new GetChildrenDepartmentsQuery(parentId, Page, Size);
}
