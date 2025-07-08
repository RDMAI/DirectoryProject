using DirectoryProject.FileService.Contracts.Dto;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;

namespace DirectoryProject.FileService.WebAPI.Features;

public sealed class GetSingleDownloadURL
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/files/downloadurl", Handler);
        }
    }

    public static async Task<IResult> Handler(
        FileLocation request,
        IS3Provider s3Provider,
        CancellationToken ct = default)
    {
        var fileURL = await s3Provider.GenerateSingleDownloadUrlAsync(request);

        return Results.Ok(fileURL);
    }
}
