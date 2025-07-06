using DirectoryProject.FileService.Contracts.Dto;
using DirectoryProject.FileService.Contracts.Requests;
using DirectoryProject.FileService.Contracts.Responses;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;

namespace DirectoryProject.FileService.WebAPI.Features;

public sealed class MultipartStartUpload
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/files/multipart/start", Handler);
        }
    }

    public const int CHUNK_SIZE = 10 * 1024 * 1024; // 10 Mb

    public static async Task<IResult> Handler(
        MultipartStartUploadRequest request,
        IS3Provider s3Provider,
        CancellationToken ct = default)
    {
        var fileId = Guid.NewGuid();

        int totalChunks = (int)(request.FileSize / CHUNK_SIZE);

        var fileLocation = new FileLocation(
                FileId: fileId.ToString(),
                BucketName: request.BucketName);

        var uploadId = await s3Provider.StartMultipartUploadAsync(
            fileName: request.FileName,
            contentType: request.ContentType,
            location: fileLocation,
            ct: ct);

        var urls = await s3Provider.GenerateAllChunkUploadUrlsAsync(
            location: fileLocation,
            uploadId: uploadId,
            totalChunks: totalChunks);

        var response = new MultipartStartUploadResponse(
            Location: fileLocation,
            UploadId: uploadId,
            ChunkSize: CHUNK_SIZE,
            TotalChunks: totalChunks,
            ChunkUploadUrls: urls);

        return Results.Ok(response);
    }
}
