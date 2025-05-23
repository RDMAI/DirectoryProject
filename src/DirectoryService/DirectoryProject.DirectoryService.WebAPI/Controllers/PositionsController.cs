using DirectoryProject.DirectoryService.Application.PositionHandlers.CreatePosition;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryProject.DirectoryService.WebAPI.Controllers;

public class PositionsController : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] ICommandHandler<CreatePositionCommand, PositionDTO> handler,
        [FromBody] CreatePositionCommand command,
        CancellationToken cancellationToken = default)
    {
        return ToAPIResponse(await handler.HandleAsync(command, cancellationToken));
    }
}
