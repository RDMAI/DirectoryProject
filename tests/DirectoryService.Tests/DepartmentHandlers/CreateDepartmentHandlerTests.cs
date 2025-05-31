using DirectoryProject.DirectoryService.Application.DepartmentHandlers.CreateDepartment;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Tests;

namespace DirectoryService.Tests.DepartmentHandlers;

public class CreateDepartmentHandlerTests : DirectoryServiceBaseHandlerTests
{
    private readonly ICommandHandler<CreateDepartmentCommand, DepartmentDTO> _sut;

    public CreateDepartmentHandlerTests(TestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<CreateDepartmentCommand, DepartmentDTO>>();
    }

    [Fact]
    public async Task HandleAsync_AddingDepartmentWithParent_ReturnSuccess()
    {
        // Arrange
        var location = Location.Create(
            id: Id<Location>.GenerateNew(),
            name: LocationName.Create("Test Location").Value,
            address: LocationAddress.Create("Moscow", "Lenina", "1").Value,
            timeZone: IANATimeZone.Create("Europe/Moscow").Value,
            createdAt: DateTime.UtcNow).Value;
        _context.Locations.Add(location);

        var parentDepartment = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Test Parent").Value,
            parent: null,
            createdAt: DateTime.UtcNow).Value;
        _context.Departments.Add(parentDepartment);

        var parentDepartmentLocation = new DepartmentLocation(
            parentDepartment.Id,
            location.Id);
        _context.DepartmentLocations.Add(parentDepartmentLocation);

        await _context.SaveChangesAsync();

        var command = new CreateDepartmentCommand(
            Name: "Test Department",
            LocationIds: [location.Id.Value],
            ParentId: parentDepartment.Id.Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsSuccess ? string.Empty : result.Errors.First().Message);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        var testName = DepartmentName.Create(command.Name).Value;
        var entity = await _context.Departments.FirstOrDefaultAsync(d => d.Name == testName);
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.IsActive, "Entity is not active");
        Assert.True(entity.ParentId is not null, "Entity's parent id is null");

        var expectedPath = Department.CreatePath(testName, parentDepartment.Path).Value;
        Assert.True(
            entity.Path == expectedPath,
            $"Entity's path is incorrect: expected {expectedPath}, got {entity.Path}");
        var expectedDepth = Department.CalculateDepth(expectedPath);
        Assert.True(
            entity.Depth == expectedDepth,
            $"Entity's depth is incorrect: expected {expectedDepth}, got {entity.Depth}");

        var expectedLocationDepartments = await _context.DepartmentLocations
            .Where(d => d.DepartmentId == entity.Id)
            .ToListAsync();
        Assert.True(
            expectedLocationDepartments.Count != 0,
            "DepartmentLocation was not created");

        Assert.True(parentDepartment.ChildrenCount == 1, "Parent's children count is 0, expected 1");
    }
}