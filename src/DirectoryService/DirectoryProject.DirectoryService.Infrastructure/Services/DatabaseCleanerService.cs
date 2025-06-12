using System.Data;
using Dapper;
using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite;
using DirectoryProject.DirectoryService.Infrastructure.Intefraces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Infrastructure.Services;

public class DatabaseCleanerService : IDatabaseCleanerService
{
    private readonly IDBConnectionFactory _connectionFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<DatabaseCleanerService> _logger;

    public DatabaseCleanerService(
        IDBConnectionFactory connectionFactory,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<DatabaseCleanerService> logger)
    {
        _connectionFactory = connectionFactory;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task CleanTablesAsync(
        TimeSpan timeToRestore,
        CancellationToken stoppingToken = default)
    {
        await ExecuteChildrenUpdateAsync(stoppingToken);

        string[] tableNames = {
                "diretory_service.departments",
                "diretory_service.positions",
                "diretory_service.locations",
            };
        using var connection = _connectionFactory.Create();
        foreach (string tableName in tableNames)
        {
            await ExecuteDeleteAsync(
                connection,
                tableName,
                timeToRestore,
                stoppingToken);
        }
    }

    private async Task ExecuteChildrenUpdateAsync(
        CancellationToken stoppingToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationWriteDBContext>();
        var repo = scope.ServiceProvider.GetRequiredService<IDepartmentRepository>();

        var deletedParents = await context.Departments
            .IgnoreQueryFilters()
            .Where(d => !d.IsActive)
            .Select(d => new {
                Path = d.Path,
                Label = d.Path.Subpath(-1) + ".",
                ParentId = d.ParentId,
            })
            .OrderByDescending(d => d.Path)
            .ToListAsync(stoppingToken);

        foreach (var parent in deletedParents)
        {
            var changedCount = await context.Departments
                .Where(d => d.IsActive)
                .Where(d => d.Path.IsDescendantOf(parent.Path))
                .Where(d => d.Path != parent.Path)
                .ExecuteUpdateAsync(
                    propCall => propCall
                    .SetProperty(
                        d => d.Path,
                        d => (LTree)((string)d.Path).Replace(parent.Label, string.Empty))
                    .SetProperty(
                        d => d.Depth,
                        d => d.Depth - 1)
                    .SetProperty(
                        d => d.ParentId,
                        d => parent.ParentId),
                    stoppingToken);
        }
    }

    private async Task<int> ExecuteDeleteAsync(
        IDbConnection connection,
        string tableName,
        TimeSpan timeToRestore,
        CancellationToken stoppingToken = default)
    {
        return await connection.ExecuteAsync(new CommandDefinition(
            commandText: $"""
                DELETE FROM {tableName}
                WHERE is_active = false AND
                    '{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}' - updated_at >= '{timeToRestore}'
                """,
            cancellationToken: stoppingToken));
    }
}
