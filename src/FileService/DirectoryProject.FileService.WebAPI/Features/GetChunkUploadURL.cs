using DirectoryProject.FileService.Contracts.Requests;
using DirectoryProject.FileService.Contracts.Responses;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;

namespace DirectoryProject.FileService.WebAPI.Features;

public sealed class GetChunkUploadURL
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/files/multipart/url", Handler);
        }
    }

    public static async Task<IResult> Handler(
        GetChunkUploadURLRequest request,
        IS3Provider s3Provider,
        CancellationToken ct = default)
    {
        if (request.PartNumber > 0)
            return Results.BadRequest("PartNumber is invalid");

        var url = await s3Provider.GenerateChunkUploadUrlAsync(
            location: request.Location,
            uploadId: request.UploadId,
            partNumber: request.PartNumber);

        return Results.Ok(new GetChunkUploadURLResponse(url, request.PartNumber));
    }
}
