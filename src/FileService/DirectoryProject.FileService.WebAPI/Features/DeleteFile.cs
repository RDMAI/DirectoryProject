using DirectoryProject.FileService.WebAPI.Domain;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;
using Framework.Helpers;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using static DirectoryProject.FileService.WebAPI.Features.GetChunkUploadURL;

namespace DirectoryProject.FileService.WebAPI.Features;

public sealed class DeleteFile
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete(
                "/api/files/{fileId:guid}",
                Handler);
        }
    }

    public static async Task<IActionResult> Handler(
        [FromRoute] Guid fileId,
        [FromQuery] string bucketName,
        IS3Provider s3Provider,
        CancellationToken ct = default)
    {
        if (fileId == Guid.Empty)
        {
            var error = ErrorHelper.General.ValueIsNullOrEmpty(
                    nameof(GetChunkUploadURLRequest.PartNumber));

            return APIResponseHelper.ToAPIResponse(error);
        }

        var deletedId = await s3Provider.DeleteFileAsync(
            location: new FileLocation(fileId.ToString(), bucketName),
            ct: ct);

        if (string.IsNullOrEmpty(deletedId))
        {
            var error = ErrorHelper.General.NotFound(fileId);
            return APIResponseHelper.ToAPIResponse(error);
        }

        var output = Result.Success(deletedId);

        return APIResponseHelper.ToAPIResponse(output);
    }
}
