using DirectoryProject.DirectoryService.Application.DepartmentHandlers.SoftDeleteDepartment;
using DirectoryProject.DirectoryService.Application.DepartmentHandlers.UpdateDepartment;
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

public class SoftDeleteDepartmentHandlerTests : DirectoryServiceBaseHandlerTests
{
    private readonly ICommandHandler<SoftDeleteDepartmentCommand> _sut;

    public SoftDeleteDepartmentHandlerTests(TestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<SoftDeleteDepartmentCommand>>();
    }

    [Fact]
    public async Task HandleAsync_DeletingDepartmentWithValidCommand_ReturnSuccess()
    {
        // Arrange
        var location = Location.Create(
            id: Id<Location>.GenerateNew(),
            name: LocationName.Create("Moscow location").Value,
            address: LocationAddress.Create("Moscow", "Lenina", "1").Value,
            timeZone: IANATimeZone.Create("Europe/Moscow").Value,
            createdAt: DateTime.UtcNow).Value;
        _context.Locations.Add(location);

        var parent = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Parent test").Value,
            parent: null,
            createdAt: DateTime.UtcNow).Value;

        var department = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create("Department test").Value,
            parent: parent,
            createdAt: DateTime.UtcNow).Value;
        _context.Departments.AddRange([parent, department]);

        _context.DepartmentLocations.AddRange([
            new DepartmentLocation(parent.Id, location.Id),
            new DepartmentLocation(department.Id, location.Id),
            ]);

        await _context.SaveChangesAsync();

        var command = new SoftDeleteDepartmentCommand(
            Id: department.Id.Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsSuccess ? string.Empty : result.Errors.First().Message);

        // check if the entity is in the database but deactivated
        var testId = Id<Department>.Create(command.Id);
        var entity = await _context.Departments
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == testId);
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.IsActive == false, "Entity is active, expected otherwise");

        var expectedLocationDepartments = await _context.DepartmentLocations
            .Where(d => d.DepartmentId == entity.Id && d.LocationId == location.Id)
            .ToListAsync();
        Assert.True(
            expectedLocationDepartments.Count != 0,
            "DepartmentLocation was deleted, expected otherwise");

        Assert.True(
            parent.ChildrenCount == 0,
            $"Parent's children count is incorrect: expected 0, got {parent.ChildrenCount}");
    }
}