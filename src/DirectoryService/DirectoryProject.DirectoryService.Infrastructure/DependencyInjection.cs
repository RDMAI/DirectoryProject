using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Infrastructure.DatabaseRead;
using DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite;
using DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryProject.DirectoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        IServiceCollection services,
        IConfiguration configuration)
    {
        DapperConfigurationHelper.Configure();

        var dbConnectionString = configuration.GetConnectionString(ApplicationWriteDBContext.DATABASE_CONFIGURATION);
        services.AddScoped(_ => new ApplicationWriteDBContext(dbConnectionString));

        services.AddScoped<IDBConnectionFactory>(_ => new ReadDBConnectionFactory(dbConnectionString));

        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();

        services.AddScoped<IUnitOfWork, DirectoryServiceUnitOfWork>();

        return services;
    }
}
