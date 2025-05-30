using DirectoryProject.DirectoryService.Application.DepartmentHandlers.UpdateDepartment;

namespace DirectoryProject.DirectoryService.WebAPI.Requests;

public record UpdateDepartmentRequest(
    string Name,
    IEnumerable<Guid> LocationIds,
    Guid? ParentId = null)
{
    public UpdateDepartmentCommand ToCommand(
        Guid id)
    {
        return new UpdateDepartmentCommand(
            id,
            Name,
            LocationIds,
            ParentId);
    }
}
