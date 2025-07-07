using Core.Abstractions;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.StartUploadLogo;

public record StartUploadLogoCommand(
    Guid DepartmentId,
    string FileName,
    string ContentType,
    long Size) : ICommand;
