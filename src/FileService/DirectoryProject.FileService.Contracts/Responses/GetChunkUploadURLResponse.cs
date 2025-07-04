namespace DirectoryProject.FileService.Contracts.Responses;

public record GetChunkUploadURLResponse(
        string URL,
        int PartNumber);
