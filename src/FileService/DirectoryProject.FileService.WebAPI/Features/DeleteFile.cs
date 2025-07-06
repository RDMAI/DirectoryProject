using DirectoryProject.FileService.Contracts.Dto;
using DirectoryProject.FileService.Contracts.Responses;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;
using Microsoft.AspNetCore.Mvc;

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

    public static async Task<IResult> Handler(
        [FromRoute] Guid fileId,
        [FromQuery] string bucketName,
        IS3Provider s3Provider,
        CancellationToken ct = default)
    {
        if (fileId == Guid.Empty)
            return Results.BadRequest("FileId is invalid");

        var deletedId = await s3Provider.DeleteFileAsync(
            location: new FileLocation(fileId.ToString(), bucketName),
            ct: ct);

        if (string.IsNullOrEmpty(deletedId))
            return Results.NotFound(deletedId);

        return Results.Ok(new DeleteFileResponse(deletedId));
    }
}
