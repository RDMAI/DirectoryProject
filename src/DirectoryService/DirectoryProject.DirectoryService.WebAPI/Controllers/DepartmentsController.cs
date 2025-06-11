using DirectoryProject.DirectoryService.Application.DepartmentHandlers.CreateDepartment;
using DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetRootDepartments;
using DirectoryProject.DirectoryService.Application.DepartmentHandlers.SoftDeleteDepartment;
using DirectoryProject.DirectoryService.Application.DepartmentHandlers.UpdateDepartment;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.WebAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
    public async Task<IActionResult> GetChildren()
    {
        //лениво загружать уровни.
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        //вернуть найденный отдел, предков и поддерево.
        return Ok();
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
