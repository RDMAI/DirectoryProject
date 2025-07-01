using Framework.Endpoints;
using Framework.Helpers;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace DirectoryProject.FileService.WebAPI.Features;

public sealed class GetDownloadURLs
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/files/urls", Handler);
        }
    }

    public static async Task<IActionResult> Handler()
    {
        return APIResponseHelper.ToAPIResponse(UnitResult.Success());
    }
}
