using Amazon.S3.Model;
using DirectoryProject.FileService.WebAPI.Domain;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;
using Framework.Helpers;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

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

    public record CompleteMultipartUploadRequest(
        FileLocation Location,
        string UploadId,
        List<PartETagModel> PartETags);

    public record CompleteMultipartUploadResponse(
        string Key);

    public static async Task<IActionResult> Handler(
        CompleteMultipartUploadRequest request,
        IS3Provider s3Provider,
        CancellationToken ct = default)
    {
        if (request.PartETags.Count == 0)
        {
            var error = ErrorHelper.General.ValueIsNullOrEmpty(
                    nameof(CompleteMultipartUploadRequest.PartETags));

            return APIResponseHelper.ToAPIResponse(error);
        }

        var key = await s3Provider.CompleteMultipartUploadAsync(
            location: request.Location,
            uploadId: request.UploadId,
            partETags: request.PartETags,
            ct: ct);

        var output = Result.Success(
            new CompleteMultipartUploadResponse(key));

        return APIResponseHelper.ToAPIResponse(output);
    }
}
