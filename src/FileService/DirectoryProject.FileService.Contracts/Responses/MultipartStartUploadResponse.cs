using DirectoryProject.FileService.Contracts.Dto;

namespace DirectoryProject.FileService.Contracts.Responses;

public record MultipartStartUploadResponse(
        FileLocation Location,
        string UploadId,
        long ChunkSize,
        int TotalChunks,
        IReadOnlyList<string> ChunkUploadUrls);
