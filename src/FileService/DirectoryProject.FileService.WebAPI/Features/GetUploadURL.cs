using DirectoryProject.FileService.Contracts.Dto;
using DirectoryProject.FileService.Contracts.Requests;
using DirectoryProject.FileService.Contracts.Responses;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;

namespace DirectoryProject.FileService.WebAPI.Features;

public sealed class GetUploadURL
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/files/url", Handler);
        }
    }

    public static async Task<IResult> Handler(
        GetUploadUrlRequest request,
        IS3Provider s3Provider,
        CancellationToken ct = default)
    {
        var key = Guid.NewGuid().ToString();

        var url = await s3Provider.GenerateUploadUrlAsync(
            fileName: request.FileName,
            location: new FileLocation(key, request.BucketName),
            ct: ct);

        var fileUrl = new FileURL(key, url);

        return Results.Ok(new GetUploadUrlResponse(fileUrl));
    }
}
