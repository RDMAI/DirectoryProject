using DirectoryProject.DirectoryService.Application.DepartmentHandlers.UpdateDepartment;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Tests;

namespace DirectoryService.Tests.DepartmentHandlers;

public class UpdateDepartmentHandlerTests : DirectoryServiceBaseHandlerTests
{
    private readonly ICommandHandler<UpdateDepartmentCommand, DepartmentDTO> _sut;

    public UpdateDepartmentHandlerTests(TestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<UpdateDepartmentCommand, DepartmentDTO>>();
    }

    [Fact]
    public async Task HandleAsync_UpdatingDepartmentWithValidCommand_ReturnSuccess()
    {
        // Arrange
        var locationBeforeUpdate = Location.Create(
            id: Id<Location>.GenerateNew(),
            name: LocationName.Create("Moscow location").Value,
            address: LocationAddress.Create("Moscow", "Lenina", "1").Value,
            timeZone: IANATimeZone.Create("Europe/Moscow").Value,
            createdAt: DateTime.UtcNow).Value;
        var locationAfterUpdate = Location.Create(
            id: Id<Location>.GenerateNew(),
            name: LocationName.Create("Saint Petersburg Location").Value,
            address: LocationAddress.Create("Saint Petersburg", "Lenina", "2").Value,
            timeZone: IANATimeZone.Create("Europe/Moscow").Value,
            createdAt: DateTime.UtcNow).Value;
        _context.Locations.AddRange([locationBeforeUpdate, locationAfterUpdate]);

        var parentBeforeUpdate = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Parent Before Update").Value,
            parent: null,
            createdAt: DateTime.UtcNow).Value;
        var parentAfterUpdate = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Parent After Update").Value,
            parent: null,
            createdAt: DateTime.UtcNow).Value;

        var departmentBeforeUpdate = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Department Before Update").Value,
            parent: parentBeforeUpdate,
            createdAt: DateTime.UtcNow).Value;
        _context.Departments.AddRange([parentBeforeUpdate, parentAfterUpdate, departmentBeforeUpdate]);

        _context.DepartmentLocations.AddRange([
            new DepartmentLocation(parentBeforeUpdate.Id, locationBeforeUpdate.Id),
            new DepartmentLocation(departmentBeforeUpdate.Id, locationBeforeUpdate.Id),
            new DepartmentLocation(parentAfterUpdate.Id, locationAfterUpdate.Id),
            ]);

        await _context.SaveChangesAsync();

        var command = new UpdateDepartmentCommand(
            Id: departmentBeforeUpdate.Id.Value,
            Name: "Department After Update",
            LocationIds: [locationAfterUpdate.Id.Value],
            ParentId: parentAfterUpdate.Id.Value);

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
        Assert.True(
            entity.ParentId is not null,
            $"Entity's parent changed to null, expected parent with id {parentAfterUpdate.Id.Value}");

        var expectedPath = Department.CreatePath(testName, parentAfterUpdate.Path).Value;
        Assert.True(
            entity.Path == expectedPath,
            $"Entity's path is incorrect: expected {expectedPath}, got {entity.Path}");
        var expectedDepth = Department.CalculateDepth(expectedPath);
        Assert.True(
            entity.Depth == expectedDepth,
            $"Entity's depth is incorrect: expected {expectedDepth}, got {entity.Depth}");

        var expectedLocationDepartments = await _context.DepartmentLocations
            .Where(d => d.DepartmentId == entity.Id && d.LocationId == locationAfterUpdate.Id)
            .ToListAsync();
        Assert.True(
            expectedLocationDepartments.Count != 0,
            "DepartmentLocation was not created");

        Assert.True(
            parentBeforeUpdate.ChildrenCount == 0,
            $"Old parent's children count is incorrect: expected 0, got {parentBeforeUpdate.ChildrenCount}");
        Assert.True(
            parentAfterUpdate.ChildrenCount == 1,
            $"New parent's children count is incorrect: expected 1, got {parentAfterUpdate.ChildrenCount}");
    }

    [Fact]
    public async Task HandleAsync_TwoConcurrentUpdatesOfDepartmentNameAndParent_MiddleOfTree_ReturnFailureAndSuccess()
    {
        // Arrange
        var location = Location.Create(
            id: Id<Location>.GenerateNew(),
            name: LocationName.Create("Moscow location").Value,
            address: LocationAddress.Create("Moscow", "Lenina", "1").Value,
            timeZone: IANATimeZone.Create("Europe/Moscow").Value,
            createdAt: DateTime.UtcNow).Value;
        _context.Locations.Add(location);

        // modeling whole tree, parents' updates also use optimistic locking
        var parentRoot = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Root parent").Value,
            parent: null,
            createdAt: DateTime.UtcNow).Value;
        var innerParent = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Parent 2 level").Value,
            parent: parentRoot,
            createdAt: DateTime.UtcNow).Value;
        var parentAfterUpdate = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Parent after update").Value,
            parent: null,
            createdAt: DateTime.UtcNow).Value;

        var departmentToUpdate = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Department Before Update").Value,
            parent: innerParent,
            createdAt: DateTime.UtcNow).Value;

        var child1 = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Child 1").Value,
            parent: departmentToUpdate,
            createdAt: DateTime.UtcNow).Value;
        var child2 = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Child 2").Value,
            parent: departmentToUpdate,
            createdAt: DateTime.UtcNow).Value;

        _context.Departments.AddRange([
            parentRoot,
            innerParent,
            parentAfterUpdate,
            departmentToUpdate,
            child1,
            child2]);

        _context.DepartmentLocations.AddRange([
            new DepartmentLocation(parentRoot.Id, location.Id),
            new DepartmentLocation(innerParent.Id, location.Id),
            new DepartmentLocation(parentAfterUpdate.Id, location.Id),
            new DepartmentLocation(departmentToUpdate.Id, location.Id),
            new DepartmentLocation(child1.Id, location.Id),
            new DepartmentLocation(child2.Id, location.Id),
            ]);

        await _context.SaveChangesAsync();

        var ct = new CancellationTokenSource().Token;

        var command1 = new UpdateDepartmentCommand(
            Id: departmentToUpdate.Id.Value,
            Name: $"Department After Update 1",
            LocationIds: [location.Id.Value],
            ParentId: parentAfterUpdate.Id.Value);
        var command2 = new UpdateDepartmentCommand(
            Id: departmentToUpdate.Id.Value,
            Name: $"Department After Update 2",
            LocationIds: [location.Id.Value],
            ParentId: parentAfterUpdate.Id.Value);

        using var scope2 = _webFactory.Services.CreateScope();
        var sut2 = scope2.ServiceProvider
            .GetRequiredService<ICommandHandler<UpdateDepartmentCommand, DepartmentDTO>>();

        // Act
        var results = await Task.WhenAll([
            _sut.HandleAsync(command1, ct),
            sut2.HandleAsync(command2, ct)
            ]);

        // Assert
        var failedResult = results.FirstOrDefault(d => d.IsFailure);
        var succededResult = results.FirstOrDefault(d => d.IsSuccess);

        Assert.True(
            failedResult is not null,
            "Both concurrent updates succeded, expected otherwise");
        Assert.True(
            succededResult is not null,
            "Both concurrent updates failed, expected otherwise");
        Assert.True(
            failedResult.Errors.First().Type == ErrorType.Conflict,
            "Both concurrent updates succeded, expected otherwise");
    }
}