using DirectoryProject.DirectoryService.Application.LocationHandlers.SoftDeleteLocation;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Tests;

namespace DirectoryService.Tests.LocationHandlers;

public class SoftDeleteLocationHandlerTests : DirectoryServiceBaseHandlerTests
{
    private readonly ICommandHandler<SoftDeleteLocationCommand> _sut;

    public SoftDeleteLocationHandlerTests(TestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<SoftDeleteLocationCommand>>();
    }

    [Fact]
    public async Task HandleAsync_DeletingLocationWithValidCommand_ReturnSuccess()
    {
        // Arrange
        var location = Location.Create(
            id: Id<Location>.GenerateNew(),
            name: LocationName.Create("Test Location").Value,
            address: LocationAddress.Create(
                city: "Moscow",
                street: "Lenina",
                houseNumber: "1").Value,
            timeZone: IANATimeZone.Create("Europe/Moscow").Value,
            createdAt: DateTime.UtcNow).Value;
        _context.Locations.Add(location);

        await _context.SaveChangesAsync();

        var command = new SoftDeleteLocationCommand(location.Id.Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsSuccess ? string.Empty : result.Errors.First().Message);

        // check if the entity is in the database but deactivated
        var testId = Id<Location>.Create(command.Id);
        var entity = await _context.Locations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == testId);
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.IsActive == false, "Entity is active, expected otherwise");
    }
}
