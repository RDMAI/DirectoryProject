namespace DirectoryProject.FileService.Contracts.Requests;

public record GetUploadUrlRequest(
        string BucketName,
        string FileName);
