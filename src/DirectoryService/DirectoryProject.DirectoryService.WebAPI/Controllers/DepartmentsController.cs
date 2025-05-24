using DirectoryProject.DirectoryService.Application.DepartmentHandlers.CreateDepartment;
using DirectoryProject.DirectoryService.Application.DepartmentHandlers.UpdateDepartment;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryProject.DirectoryService.WebAPI.Controllers;

public class DepartmentsController : ApplicationController
{
    [HttpGet("roots")]
    public async Task<IActionResult> GetRoots()
    {
        //страницы корней + prefetch детей.
        return Ok();
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
        [FromBody] UpdateDepartmentCommand command,
        CancellationToken cancellationToken = default)
    {
        return ToAPIResponse(await handler.HandleAsync(command, cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete()
    {
        //soft-delete отдела
        //    запрет, если есть активные потомки.
        return Ok();
    }
}
