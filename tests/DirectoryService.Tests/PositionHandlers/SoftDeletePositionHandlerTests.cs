using DirectoryProject.DirectoryService.Application.LocationHandlers.SoftDeleteLocation;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using DirectoryProject.DirectoryService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectoryProject.DirectoryService.Application.PositionHandlers.SoftDeletePosition;
using DirectoryProject.DirectoryService.Domain.PositionValueObjects;

namespace DirectoryService.Tests.PositionHandlers;

public class SoftDeletePositionHandlerTests : DirectoryServiceBaseHandlerTests
{
    private readonly ICommandHandler<SoftDeletePositionCommand> _sut;

    public SoftDeletePositionHandlerTests(TestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<SoftDeletePositionCommand>>();
    }

    [Fact]
    public async Task HandleAsync_DeletingPositionWithValidCommand_ReturnSuccess()
    {
        // Arrange
        var position = Position.Create(
            id: Id<Position>.GenerateNew(),
            name: PositionName.Create("Test Position").Value,
            description: PositionDescription.Create("Test description").Value,
            createdAt: DateTime.UtcNow).Value;
        _context.Positions.Add(position);

        await _context.SaveChangesAsync();

        var command = new SoftDeletePositionCommand(position.Id.Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsSuccess ? string.Empty : result.Errors.First().Message);

        // check if the entity is in the database but deactivated
        var testId = Id<Position>.Create(command.Id);
        var entity = await _context.Positions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == testId);
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.IsActive == false, "Entity is active, expected otherwise");
    }
}
