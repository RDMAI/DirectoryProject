using DirectoryProject.FileService.WebAPI.Domain;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;
using Framework.Helpers;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace DirectoryProject.FileService.WebAPI.Features;

public sealed class MultipartCancelUpload
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/files/multipart/cancel", Handler);
        }
    }

    public record MultipartCancelUploadRequest(
        FileLocation Location,
        string UploadId);

    public static async Task<IActionResult> Handler(
        MultipartCancelUploadRequest request,
        IS3Provider s3Provider,
        CancellationToken ct = default)
    {
        await s3Provider.AbortMultipartUploadAsync(
            request.Location,
            request.UploadId,
            ct: ct);

        return APIResponseHelper.ToAPIResponse(UnitResult.Success());
    }
}
