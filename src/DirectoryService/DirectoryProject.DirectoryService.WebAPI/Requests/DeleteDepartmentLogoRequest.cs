using DirectoryProject.DirectoryService.Application.DepartmentHandlers.DeleteLogo;

namespace DirectoryProject.DirectoryService.WebAPI.Requests;

public record DeleteDepartmentLogoRequest(
    string FileId)
{
    public DeleteLogoCommand ToCommand(
        Guid id)
    {
        return new DeleteLogoCommand(
            id,
            FileId);
    }
}
