using DirectoryProject.DirectoryService.Application.PositionHandlers.CreatePosition;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
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
        var command = new CreatePositionCommand(
            Name: "Test Position",
            Description: "Test description");

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
    }
}
