using DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetRootDepartments;
using DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocationById;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Tests;

namespace DirectoryService.Tests.DepartmentHandlers;

public class GetRootDepartmentsHandlerTests : DirectoryServiceBaseHandlerTests
{
    private readonly IQueryHandler<GetRootDepartmentsQuery, FilteredListDTO<DepartmentTreeDTO>> _sut;

    public GetRootDepartmentsHandlerTests(TestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandler<GetRootDepartmentsQuery, FilteredListDTO<DepartmentTreeDTO>>>();
    }

    [Fact]
    public async Task HandleAsync_GettingFirstPageOfSize3_ReturnSuccess3Locations()
    {
        //// Arrange
        //var query = new GetLocationByIdQuery(location.Id.Value);

        //var ct = new CancellationTokenSource().Token;

        //// Act
        //var result = await _sut.HandleAsync(query, ct);

        //// Assert
    }
}
