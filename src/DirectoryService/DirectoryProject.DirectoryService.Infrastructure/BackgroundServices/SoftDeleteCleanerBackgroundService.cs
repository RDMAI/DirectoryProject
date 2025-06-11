using Dapper;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DirectoryProject.DirectoryService.Infrastructure.BackgroundServices;

public class SoftDeleteCleanerBackgroundService : BackgroundService
{
    private readonly IDBConnectionFactory _connectionFactory;
    private readonly ILogger<SoftDeleteCleanerBackgroundService> _logger;

    private readonly TimeSpan _checkPeriod;
    private readonly TimeSpan _timeToRestore;

    public SoftDeleteCleanerBackgroundService(
        IDBConnectionFactory connectionFactory,
        ILogger<SoftDeleteCleanerBackgroundService> logger,
        IOptions<SoftDeleteCleanerOptions> options)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;

        if (options != null)
        {
            _checkPeriod = TimeSpan.FromHours(options.Value.CheckPeriodHours);
            _timeToRestore = TimeSpan.FromHours(options.Value.TimeToRestoreHours);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SoftDeleteCleanerBackgroundService started");

        using PeriodicTimer timer = new(_checkPeriod);

        // when stop requested, waiting for timer to tick will be cancelled too
        while (!stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var connection = _connectionFactory.Create();

                var departmentDeleteResult = await connection.ExecuteAsync(new CommandDefinition(
                    $"""
                    DELETE FROM diretory_service.departments
                    WHERE is_active = false AND
                        '{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}' - updated_at >= '{_timeToRestore}'
                    """,
                    cancellationToken: stoppingToken));

                var locationDeleteResult = await connection.ExecuteAsync(new CommandDefinition(
                    $"""
                    DELETE FROM diretory_service.locations
                    WHERE is_active = false AND
                        '{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}' - updated_at >= '{_timeToRestore}'
                    """,
                    cancellationToken: stoppingToken));

                var positionDeleteResult = await connection.ExecuteAsync(new CommandDefinition(
                    $"""
                    DELETE FROM diretory_service.positions
                    WHERE is_active = false AND
                        '{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}' - updated_at >= '{_timeToRestore}'
                    """,
                    cancellationToken: stoppingToken));

                _logger.LogInformation("SoftDeleteCleanerBackgroundService executed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        _logger.LogInformation("SoftDeleteCleanerBackgroundService finished");
    }

    public class SoftDeleteCleanerOptions
    {
        public required float CheckPeriodHours { get; set; }
        public required float TimeToRestoreHours { get; set; }
    }
}
