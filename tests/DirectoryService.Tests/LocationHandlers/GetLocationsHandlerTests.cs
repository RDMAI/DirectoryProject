using DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocations;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Shared.Tests;

namespace DirectoryService.Tests.LocationHandlers;

public class GetLocationsHandlerTests : DirectoryServiceBaseHandlerTests
{
    private readonly IQueryHandler<GetLocationsQuery, FilteredListDTO<LocationDTO>> _sut;

    public GetLocationsHandlerTests(TestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandler<GetLocationsQuery, FilteredListDTO<LocationDTO>>>();
    }

    [Fact]
    public async Task HandleAsync_GettingFirstPageOfSize3FilteredByName_ReturnSuccess3Locations()
    {
        // Arrange
        for (int i = 0; i < 21; i++)
        {
            _context.Locations.Add(
                Location.Create(
                    id: Id<Location>.GenerateNew(),
                    name: LocationName.Create($"Test Location {i}").Value,
                    address: LocationAddress.Create(
                        city: "Moscow",
                        street: "Lenina",
                        houseNumber: "1").Value,
                    timeZone: IANATimeZone.Create("Europe/Moscow").Value,
                    createdAt: DateTime.UtcNow).Value);
        }

        await _context.SaveChangesAsync();

        var query = new GetLocationsQuery(
            Page: 1,
            Size: 3,
            Search: "1");

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(query, ct);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsSuccess ? string.Empty : result.Errors.First().Message);
        Assert.True(result.Value is not null, "Result value is null");

        var resultList = result.Value.Data;

        Assert.True(
            resultList.Count() == query.Size,
            $"Items count is incorrect. Expected {query.Size}, got {resultList.Count()}");
        // number 1 and from 10 to 19
        Assert.True(
            result.Value.Total == 11,
            $"Total count is incorrect. Expected {11}, got {result.Value.Total}");
    }
}
