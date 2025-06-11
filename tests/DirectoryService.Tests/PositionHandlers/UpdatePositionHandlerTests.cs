using DirectoryProject.DirectoryService.Application.PositionHandlers.UpdatePosition;
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

public class UpdatePositionHandlerTests : DirectoryServiceBaseHandlerTests
{
    private readonly ICommandHandler<UpdatePositionCommand, PositionDTO> _sut;

    public UpdatePositionHandlerTests(TestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<UpdatePositionCommand, PositionDTO>>();
    }

    [Fact]
    public async Task HandleAsync_UpdatingPositionWithValidCommand_ReturnSuccess()
    {
        // Arrange
        var position = Position.Create(
            id: Id<Position>.GenerateNew(),
            name: PositionName.Create("Test Position").Value,
            description: PositionDescription.Create("Test description").Value,
            createdAt: DateTime.UtcNow).Value;
        _context.Positions.Add(position);

        var dep1 = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Test dep1").Value,
            parent: null,
            createdAt: DateTime.UtcNow).Value;
        var dep2 = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Test dep2").Value,
            parent: null,
            createdAt: DateTime.UtcNow).Value;
        _context.Departments.AddRange([dep1, dep2]);

        _context.DepartmentPositions.Add(
            new DepartmentPosition(dep1.Id, position.Id));

        await _context.SaveChangesAsync();

        var command = new UpdatePositionCommand(
            Id: position.Id.Value,
            Name: "Test Position after update",
            Description: "Test description after update",
            DepartmentIds: [dep2.Id.Value]);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsSuccess ? string.Empty : result.Errors.First().Message);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        var entity = await _context.Positions.FirstOrDefaultAsync(d => d.Id == position.Id);
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.IsActive, "Entity is not active");
        Assert.True(
            entity.Name.Value == command.Name,
            $"Entity's name is incorrent. Expected {command.Name}, got {entity.Name}");

        var expectedDepartmentPositions = await _context.DepartmentPositions
            .Where(d => d.PositionId == entity.Id && d.DepartmentId == dep2.Id)
            .ToListAsync();
        Assert.True(
            expectedDepartmentPositions.Count != 0,
            "DepartmentPosition was not created");
    }
}
