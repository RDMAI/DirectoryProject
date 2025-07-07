using DirectoryProject.FileService.Contracts.Dto;

namespace DirectoryProject.FileService.Contracts.Responses;

public record CompleteMultipartUploadResponse(
    string Key,
    FileMetadata Metadata);
