using DirectoryProject.DirectoryService.Application.LocationHandlers.CreateLocation;
using DirectoryProject.DirectoryService.Application.LocationHandlers.UpdateLocation;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryService.Tests.LocationHandlers;

public class UpdateLocationHandlerTests : DirectoryServiceBaseHandlerTests
{
    private readonly ICommandHandler<UpdateLocationCommand, LocationDTO> _sut;

    public UpdateLocationHandlerTests(TestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<UpdateLocationCommand, LocationDTO>>();
    }

    [Fact]
    public async Task HandleAsync_UpdatingLocationWithValidCommand_ReturnSuccess()
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

        var command = new UpdateLocationCommand(
            Id: location.Id.Value,
            Name: "Test Location after update",
            Address: new AddressDTO(
                City: "Saint Petersburg",
                Street: "Lenina",
                HouseNumber: "2"),
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
        var entity = await _context.Locations.FirstOrDefaultAsync(d => d.Id == location.Id);
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.IsActive, "Entity is not active");
        Assert.True(
            entity.Name.Value == command.Name,
            $"Entity's name is incorrent. Expected {command.Name}, got {entity.Name}");
    }
}
