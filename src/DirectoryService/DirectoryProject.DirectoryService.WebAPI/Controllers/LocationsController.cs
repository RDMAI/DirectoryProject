using DirectoryProject.DirectoryService.Application.LocationHandlers.CreateLocation;
using DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocationById;
using DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocations;
using DirectoryProject.DirectoryService.Application.LocationHandlers.SoftDeleteLocation;
using DirectoryProject.DirectoryService.Application.LocationHandlers.UpdateLocation;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.WebAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DirectoryProject.DirectoryService.WebAPI.Controllers;

public class LocationsController : ApplicationController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] IQueryHandler<GetLocationsQuery, FilteredListDTO<LocationDTO>> handler,
        [FromQuery] GetLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        return ToAPIResponse(await handler.HandleAsync(query, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAll(
        [FromServices] IQueryHandler<GetLocationByIdQuery, LocationDTO> handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        return ToAPIResponse(await handler.HandleAsync(new GetLocationByIdQuery(id), cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] ICommandHandler<CreateLocationCommand, LocationDTO> handler,
        [FromBody] CreateLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        return ToAPIResponse(await handler.HandleAsync(command, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromServices] ICommandHandler<UpdateLocationCommand, LocationDTO> handler,
        [FromBody] UpdateLocationRequest request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        return ToAPIResponse(await handler.HandleAsync(request.ToCommand(id), cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromServices] ICommandHandler<SoftDeleteLocationCommand> handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new SoftDeleteLocationCommand(id);

        return ToAPIResponse(await handler.HandleAsync(command, cancellationToken));
    }
}
