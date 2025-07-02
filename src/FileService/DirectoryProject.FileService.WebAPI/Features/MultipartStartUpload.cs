using DirectoryProject.FileService.WebAPI.Domain;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;
using Framework.Helpers;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

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

    public record MultipartStartUploadRequest(
        string FileName,
        string ContentType,
        long FileSize,
        string BucketName);

    public record MultipartStartUploadResponse(
        FileLocation Location,
        string UploadId,
        long ChunkSize,
        int TotalChunks,
        IReadOnlyList<string> ChunkUploadUrls);

    public const int CHUNK_SIZE = 10 * 1024 * 1024; // 10 Mb

    public static async Task<IActionResult> Handler(
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

        var output = Result.Success(response);

        return APIResponseHelper.ToAPIResponse(output);
    }
}
