using DirectoryProject.FileService.Contracts.Requests;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;

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

    public static async Task<IResult> Handler(
        MultipartCancelUploadRequest request,
        IS3Provider s3Provider,
        CancellationToken ct = default)
    {
        await s3Provider.AbortMultipartUploadAsync(
            request.Location,
            request.UploadId,
            ct: ct);

        return Results.Ok(string.Empty);
    }
}
