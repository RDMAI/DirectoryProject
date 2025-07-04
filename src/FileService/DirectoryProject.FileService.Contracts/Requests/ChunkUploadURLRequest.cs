using DirectoryProject.FileService.Contracts.Dto;

namespace DirectoryProject.FileService.Contracts.Requests;

public record GetChunkUploadURLRequest(
        FileLocation Location,
        string UploadId,
        int PartNumber);
