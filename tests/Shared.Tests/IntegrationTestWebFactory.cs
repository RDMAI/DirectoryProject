using DirectoryProject.DirectoryService.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;

namespace Shared.Tests;

public class TestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected string _connectionString = string.Empty;

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("directory_service_tests")
        .WithUsername("postgresUser")
        .WithPassword("postgresPassword")
        .Build();

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();

        var accountContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        await accountContext.Database.EnsureDeletedAsync();

        await accountContext.Database.EnsureCreatedAsync();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _connectionString = _dbContainer.GetConnectionString();

        await ResetDatabaseAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(ReplaceServices);
    }

    protected virtual void ReplaceServices(IServiceCollection services)
    {
        services.RemoveAll<ApplicationDBContext>();
        services.AddScoped(_ => new ApplicationDBContext(_connectionString));
    }
}
