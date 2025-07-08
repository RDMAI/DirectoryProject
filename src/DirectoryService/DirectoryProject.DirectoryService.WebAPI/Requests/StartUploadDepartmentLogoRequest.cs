using DirectoryProject.DirectoryService.Application.DepartmentHandlers.StartUploadLogo;
using DirectoryProject.DirectoryService.Application.DepartmentHandlers.UpdateDepartment;

namespace DirectoryProject.DirectoryService.WebAPI.Requests;

public record StartUploadDepartmentLogoRequest(
    string FileName,
    string ContentType,
    long Size)
{
    public StartUploadLogoCommand ToCommand(
        Guid id)
    {
        return new StartUploadLogoCommand(
            id,
            FileName,
            ContentType,
            Size);
    }
}
