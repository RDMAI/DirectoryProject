using DirectoryProject.FileService.Contracts.Requests;
using DirectoryProject.FileService.Contracts.Responses;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;

namespace DirectoryProject.FileService.WebAPI.Features;

public sealed class MultipartCompleteUpload
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/files/multipart/end", Handler);
        }
    }

    public static async Task<IResult> Handler(
        CompleteMultipartUploadRequest request,
        IS3Provider s3Provider,
        CancellationToken ct = default)
    {
        if (request.PartETags.Count == 0)
            return Results.BadRequest("PartETags list is empty");

        var key = await s3Provider.CompleteMultipartUploadAsync(
            location: request.Location,
            uploadId: request.UploadId,
            partETags: request.PartETags,
            ct: ct);

        return Results.Ok(new CompleteMultipartUploadResponse(key));
    }
}
