using DirectoryProject.FileService.Contracts.Dto;

namespace DirectoryProject.FileService.Contracts.Requests;

public record MultipartCancelUploadRequest(
        FileLocation Location,
        string UploadId);
