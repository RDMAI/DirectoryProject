using DirectoryProject.FileService.WebAPI.Domain;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;
using Framework.Helpers;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

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

    public record GetUploadUrlRequest(
        string BucketName,
        string FileName);

    public record GetUploadUrlResponse(
        FileURL URL);

    public static async Task<IActionResult> Handler(
        GetUploadUrlRequest request,
        IS3Provider s3Provider,
        CancellationToken ct = default)
    {
        var key = Guid.NewGuid().ToString();

        var url = await s3Provider.GenerateUploadUrlAsync(
            fileName: request.FileName,
            location: new FileLocation(key, request.BucketName),
            ct: ct);

        var output = Result.Success(new FileURL(key, url));

        return APIResponseHelper.ToAPIResponse(output);
    }
}
