using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Infrastructure.BackgroundServices;
using DirectoryProject.DirectoryService.Infrastructure.DatabaseRead;
using DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite;
using DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite.Repositories;
using DirectoryProject.DirectoryService.Infrastructure.Intefraces;
using DirectoryProject.DirectoryService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static DirectoryProject.DirectoryService.Infrastructure.BackgroundServices.SoftDeleteCleanerBackgroundService;

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

        services.AddSingleton<IDBConnectionFactory>(_ => new ReadDBConnectionFactory(dbConnectionString));

        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();

        services.AddScoped<IUnitOfWork, DirectoryServiceUnitOfWork>();

        services.AddSingleton<IDatabaseCleanerService, DatabaseCleanerService>();

        var configs = configuration.GetSection("SoftDeleteCleaner");
        services.Configure<SoftDeleteCleanerOptions>(configs);
        services.AddHostedService<SoftDeleteCleanerBackgroundService>();

        return services;
    }
}
