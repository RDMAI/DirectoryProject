using Core.Abstractions;
using DirectoryProject.DirectoryService.Application.DTOs;
using DirectoryProject.DirectoryService.Application.PositionHandlers.CreatePosition;
using DirectoryProject.DirectoryService.Application.PositionHandlers.SoftDeletePosition;
using DirectoryProject.DirectoryService.Application.PositionHandlers.UpdatePosition;
using DirectoryProject.DirectoryService.WebAPI.Requests;
using Framework;
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromServices] ICommandHandler<UpdatePositionCommand, PositionDTO> handler,
        [FromBody] UpdatePositionRequest request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        return ToAPIResponse(await handler.HandleAsync(request.ToCommand(id), cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromServices] ICommandHandler<SoftDeletePositionCommand> handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new SoftDeletePositionCommand(id);

        return ToAPIResponse(await handler.HandleAsync(command, cancellationToken));
    }
}
