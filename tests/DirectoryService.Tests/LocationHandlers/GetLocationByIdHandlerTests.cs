using DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocationById;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Shared.Tests;

namespace DirectoryService.Tests.LocationHandlers;

public class GetLocationByIdHandlerTests : DirectoryServiceBaseHandlerTests
{
    private readonly IQueryHandler<GetLocationByIdQuery, LocationDTO> _sut;

    public GetLocationByIdHandlerTests(TestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandler<GetLocationByIdQuery, LocationDTO>>();
    }

    [Fact]
    public async Task HandleAsync_GettingLocationWithValidQuery_ReturnSuccess()
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

        var query = new GetLocationByIdQuery(location.Id.Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(query, ct);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsSuccess ? string.Empty : result.Errors.First().Message);
        Assert.True(result.Value is not null, "Result value is null");
    }
}
