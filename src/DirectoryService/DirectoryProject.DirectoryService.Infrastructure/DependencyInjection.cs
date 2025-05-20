using DirectoryProject.DirectoryService.Infrastructure.Database;
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

        return services;
    }
}
