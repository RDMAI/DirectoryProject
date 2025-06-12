using DirectoryProject.DirectoryService.Application.LocationHandlers.CreateLocation;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Tests;

namespace DirectoryService.Tests.LocationHandlers;

public class CreateLocationHandlerTests : DirectoryServiceBaseHandlerTests
{
    private readonly ICommandHandler<CreateLocationCommand, LocationDTO> _sut;

    public CreateLocationHandlerTests(TestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<CreateLocationCommand, LocationDTO>>();
    }

    [Fact]
    public async Task HandleAsync_AddingLocationWithValidCommand_ReturnSuccess()
    {
        // Arrange
        var command = new CreateLocationCommand(
            Name: "Test Location",
            Address: new AddressDTO(
                City: "Moscow",
                Street: "Lenina",
                HouseNumber: "1"),
            TimeZone: "Europe/Moscow");

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsSuccess ? string.Empty : result.Errors.First().Message);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        var testName = LocationName.Create(command.Name).Value;
        var entity = await _context.Locations.FirstOrDefaultAsync(d => d.Name == testName);
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.IsActive, "Entity is not active");
    }
}
