using DirectoryProject.DirectoryService.Application.PositionHandlers.CreatePosition;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Tests;

namespace DirectoryService.Tests.PositionHandlers;

public class CreatePositionHandlerTests : DirectoryServiceBaseHandlerTests
{
    private readonly ICommandHandler<CreatePositionCommand, PositionDTO> _sut;

    public CreatePositionHandlerTests(TestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<CreatePositionCommand, PositionDTO>>();
    }

    [Fact]
    public async Task HandleAsync_AddingPositionWithValidCommand_ReturnSuccess()
    {
        // Arrange
        var dep = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Test dep").Value,
            parent: null,
            createdAt: DateTime.UtcNow).Value;
        _context.Departments.Add(dep);

        var command = new CreatePositionCommand(
            Name: "Test Position",
            Description: "Test description",
            DepartmentIds: [dep.Id.Value]);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsSuccess ? string.Empty : result.Errors.First().Message);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        var testName = PositionName.Create(command.Name).Value;
        var entity = await _context.Positions.FirstOrDefaultAsync(d => d.Name == testName);
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.IsActive, "Entity is not active");
        Assert.True(entity.DepartmentPositions.Any(), "Position is not added to Department");
    }
}
