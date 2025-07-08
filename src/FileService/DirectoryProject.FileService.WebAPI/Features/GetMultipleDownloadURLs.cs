using DirectoryProject.FileService.Contracts.Requests;
using DirectoryProject.FileService.Contracts.Responses;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;

namespace DirectoryProject.FileService.WebAPI.Features;

public sealed class GetMultipleDownloadURLs
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/files/downloadurls", Handler);
        }
    }

    public static async Task<IResult> Handler(
        GetDownloadURLsRequest request,
        IS3Provider s3Provider,
        CancellationToken ct = default)
    {
        if (request.Locations.Count <= 0)
            return Results.BadRequest("Location list is empty");

        var urls = await s3Provider.GenerateDownloadUrlsAsync(
            locations: request.Locations);

        return Results.Ok(new GetDownloadURLsResponse(urls));
    }
}
