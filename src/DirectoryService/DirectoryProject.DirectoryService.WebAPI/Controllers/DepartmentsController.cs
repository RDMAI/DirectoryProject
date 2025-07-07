using Core.Abstractions;
using DirectoryProject.DirectoryService.Application.DepartmentHandlers.CreateDepartment;
using DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetChildrenDepartments;
using DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetRootDepartments;
using DirectoryProject.DirectoryService.Application.DepartmentHandlers.SoftDeleteDepartment;
using DirectoryProject.DirectoryService.Application.DepartmentHandlers.UpdateDepartment;
using DirectoryProject.DirectoryService.Application.DTOs;
using DirectoryProject.DirectoryService.WebAPI.Requests;
using DirectoryProject.FileService.Communication;
using Framework;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryProject.DirectoryService.WebAPI.Controllers;

public class DepartmentsController : ApplicationController
{
    [HttpGet("roots")]
    public async Task<IActionResult> GetRoots(
        [FromServices] IQueryHandler<GetRootDepartmentsQuery, FilteredListDTO<DepartmentTreeDTO>> handler,
        [FromQuery] GetRootDepartmentsQuery query,
        CancellationToken cancellationToken = default)
    {
        return ToAPIResponse(await handler.HandleAsync(query, cancellationToken));
    }

    [HttpGet("{id:guid}/children")]
    public async Task<IActionResult> GetChildren(
        [FromServices] IQueryHandler<GetChildrenDepartmentsQuery, FilteredListDTO<DepartmentDTO>> handler,
        [FromRoute] Guid id,
        [FromQuery] GetChildrenDepartmentsRequest request,
        CancellationToken cancellationToken = default)
    {
        return ToAPIResponse(await handler.HandleAsync(request.ToQuery(id), cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] ICommandHandler<CreateDepartmentCommand, DepartmentDTO> handler,
        [FromBody] CreateDepartmentCommand command,
        CancellationToken cancellationToken = default)
    {
        return ToAPIResponse(await handler.HandleAsync(command, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromServices] ICommandHandler<UpdateDepartmentCommand, DepartmentDTO> handler,
        [FromBody] UpdateDepartmentRequest request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        return ToAPIResponse(await handler.HandleAsync(request.ToCommand(id), cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromServices] ICommandHandler<SoftDeleteDepartmentCommand> handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new SoftDeleteDepartmentCommand(id);

        return ToAPIResponse(await handler.HandleAsync(command, cancellationToken));
    }
}
