using System.Data;
using DirectoryProject.DirectoryService.Domain.Shared;

namespace DirectoryProject.DirectoryService.Infrastructure.Intefraces;

public interface IDatabaseCleanerService
{
    Task CleanTablesAsync(
        TimeSpan timeToRestore,
        CancellationToken stoppingToken = default);
}
