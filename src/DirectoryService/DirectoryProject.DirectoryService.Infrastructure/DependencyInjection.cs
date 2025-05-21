using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Infrastructure.Database;
using DirectoryProject.DirectoryService.Infrastructure.Database.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryProject.DirectoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var dbConnectionString = configuration.GetConnectionString(ApplicationDBContext.DATABASE_CONFIGURATION);
        services.AddScoped(_ => new ApplicationDBContext(dbConnectionString));

        services.AddScoped<ILocationRepository, LocationRepository>();

        return services;
    }
}
