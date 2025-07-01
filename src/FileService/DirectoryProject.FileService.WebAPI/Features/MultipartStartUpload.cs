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
            app.MapPost("/api/files/urls", Handler);
        }
    }

    public record MultipartStartUploadRequest(
        string FileName,
        string ContentType,
        long FileSize,
        string BucketName);

    public record MultipartStartUploadResponse(
        string FileId,
        string UploadId,
        string BucketName,
        long ChunkSize,
        int TotalChunks,
        IReadOnlyList<ChunkUploadURL> ChunkUploadUrls);

    public static async Task<IActionResult> Handler(
        MultipartStartUploadRequest request)
    {
        return APIResponseHelper.ToAPIResponse(UnitResult.Success());
    }
}
