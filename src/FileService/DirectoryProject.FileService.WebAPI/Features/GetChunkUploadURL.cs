using DirectoryProject.FileService.WebAPI.Domain;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;
using Framework.Helpers;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

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

    public record GetChunkUploadURLRequest(
        FileLocation Location,
        string UploadId,
        int PartNumber);

    public record GetChunkUploadURLResponse(
        string URL,
        int PartNumber);

    public static async Task<IActionResult> Handler(
        GetChunkUploadURLRequest request,
        IS3Provider s3Provider,
        CancellationToken ct = default)
    {
        if (request.PartNumber > 0)
        {
            var error = ErrorHelper.General.ValueIsNullOrEmpty(
                    nameof(GetChunkUploadURLRequest.PartNumber));

            return APIResponseHelper.ToAPIResponse(error);
        }

        var url = await s3Provider.GenerateChunkUploadUrlAsync(
            location: request.Location,
            uploadId: request.UploadId,
            partNumber: request.PartNumber);

        var output = Result.Success(
            new GetChunkUploadURLResponse(url, request.PartNumber));

        return APIResponseHelper.ToAPIResponse(output);
    }
}
