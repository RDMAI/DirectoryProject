using DirectoryProject.DirectoryService.Application.DepartmentHandlers.CompleteUploadLogo;
using DirectoryProject.FileService.Contracts.Dto;

namespace DirectoryProject.DirectoryService.WebAPI.Requests;

public record CompleteUploadDepartmentLogoRequest(
    string FileId,
    string UploadId,
    List<PartETagModel> PartETags)
{
    public CompleteUploadLogoCommand ToCommand(
        Guid id)
    {
        return new CompleteUploadLogoCommand(
            id,
            FileId,
            UploadId,
            PartETags);
    }
}
