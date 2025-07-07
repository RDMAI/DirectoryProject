using Core.Abstractions;
using DirectoryProject.FileService.Contracts.Dto;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.CompleteUploadLogo;

public record CompleteUploadLogoCommand(
    Guid DepartmentId,
    string FileId,
    string UploadId,
    List<PartETagModel> PartETags) : ICommand;
