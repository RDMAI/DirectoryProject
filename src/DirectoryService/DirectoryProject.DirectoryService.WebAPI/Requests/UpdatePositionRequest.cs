using DirectoryProject.DirectoryService.Application.PositionHandlers.UpdatePosition;

namespace DirectoryProject.DirectoryService.WebAPI.Requests;

public record UpdatePositionRequest(
    string Name,
    string Description,
    IEnumerable<Guid> DepartmentIds)
{
    public UpdatePositionCommand ToCommand(
        Guid id)
    {
        return new UpdatePositionCommand(id, Name, Description, DepartmentIds);
    }
}
