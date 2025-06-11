//using Dapper;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//namespace DirectoryProject.DirectoryService.Infrastructure.BackgroundServices;

//public class SoftDeleteCleanerBackgroundService : BackgroundService
//{
//    private readonly IServiceScope _serviceScope;
//    private readonly ILogger<SoftDeleteCleanerBackgroundService> _logger;
//    private readonly IDBConnectionFactory _dBConnectionFactory;

//    private readonly TimeSpan _checkPeriod;
//    private readonly TimeSpan _timeToRestore;

//    public SoftDeleteCleanerBackgroundService(
//        IDbContextFactory<PetsWriteDBContext> dbFactory,
//        ILogger<SoftDeleteCleanerBackgroundService> logger,
//        [FromKeyedServices(DependencyKey.Pets)] IDBConnectionFactory dBConnectionFactory,
//        IMessageQueue<IEnumerable<FileInfoDTO>> fileMessageQueue,
//        IOptions<SoftDeleteCleanerOptions> options)
//    {
//        _dbFactory = dbFactory;
//        _logger = logger;
//        _dBConnectionFactory = dBConnectionFactory;
//        _fileMessageQueue = fileMessageQueue;
//        if (options != null)
//        {
//            _checkPeriod = TimeSpan.FromHours(options.Value.CheckPeriodHours);
//            _timeToRestore = TimeSpan.FromHours(options.Value.TimeToRestoreHours);
//        }
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        _logger.LogInformation("SoftDeleteCleanerBackgroundService started");

//        using PeriodicTimer timer = new(_checkPeriod);

//        // when stop requested, waiting for timer to tick will be cancelled too
//        while (!stoppingToken.IsCancellationRequested &&
//            await timer.WaitForNextTickAsync(stoppingToken))
//        {
//            try
//            {
//                using var context = _dbFactory.CreateDbContext();

//                // deleting pets
//                await _deletePets(context, stoppingToken);

//                // deleting volunteers
//                var deletedVolunteers = context.Volunteers
//                    .Where(v => v.IsDeleted &&
//                        DateTime.UtcNow - v.DeletionDate!.Value >= _timeToRestore);

//                // this will also cascade delete related pets
//                context.Volunteers.RemoveRange(deletedVolunteers);
//                await context.SaveChangesAsync(stoppingToken);

//                _logger.LogInformation("SoftDeleteCleanerBackgroundService executed successfully");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, ex.Message);
//            }
//        }

//        _logger.LogInformation("SoftDeleteCleanerBackgroundService finished");
//    }

//    private class _petPhotosDTO
//    {
//        public Guid Id { get; init; }
//        public FileVO[] Photos { get; init; } = [];
//    }

//    private async Task _deletePets(PetsWriteDBContext context, CancellationToken stoppingToken)
//    {
//        using var connection = _dBConnectionFactory.Create();

//        // Getting all required to delete pets. Retrieving id and file paths
//        var petsSql = $"""
//            SELECT id, photos
//            FROM pets_management.Pets
//            WHERE is_deleted AND
//                '{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}' - deletion_date >= '{_timeToRestore}'
//            """;

//        _logger.LogInformation("Dapper pets select sql:\n{sql}", petsSql);
//        var pets = await connection.QueryAsync<_petPhotosDTO>(petsSql);
//        if (pets is null || pets.Any() == false)
//            return;

//        // Creating a list of ids for postgres
//        var deletedPetIds = "('" + string.Join("', '", pets.Select(p => p.Id)) + "')";

//        var petsDeleteSQL = $"""
//            DELETE FROM pets_management.Pets
//            WHERE id IN {deletedPetIds}
//            """;
//        _logger.LogInformation("Pets delete sql:\n{sql}", petsSql);

//        // With raw sql it is one database call. With DDD, it will be 2 (get volunteers and update volunteers)
//        var deleteResult = await context.Database.ExecuteSqlRawAsync(petsDeleteSQL, stoppingToken);

//        // Placing an operation to delete photos of deleted pets
//        List<FileInfoDTO> fileInfos = [];
//        foreach (var p in pets)
//        {
//            var f = p.Photos.Select(photo => new FileInfoDTO(
//                photo.PathToStorage,
//                Constants.BucketNames.PET_PHOTOS));
//            fileInfos.AddRange(f);
//        }
//        if (fileInfos.Count != 0)
//            await _fileMessageQueue.WriteAsync(fileInfos, stoppingToken);
//    }
//}
