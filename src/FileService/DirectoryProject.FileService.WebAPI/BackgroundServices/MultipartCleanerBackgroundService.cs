using DirectoryProject.FileService.Contracts.Dto;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Microsoft.Extensions.Options;

namespace DirectoryProject.FileService.WebAPI.BackgroundServices;

public class MultipartCleanerBackgroundService : BackgroundService
{
    private readonly ILogger<MultipartCleanerBackgroundService> _logger;
    private readonly IS3Provider _s3Provider;

    private readonly TimeSpan _checkPeriod;
    private readonly TimeSpan _timeout;

    public MultipartCleanerBackgroundService(
        ILogger<MultipartCleanerBackgroundService> logger,
        IS3Provider s3Provider,
        IOptions<MultipartCleanerOptions> options)
    {
        _logger = logger;
        _s3Provider = s3Provider;

        if (options != null)
        {
            _checkPeriod = TimeSpan.FromMinutes(options.Value.CheckPeriodMinutes);
            _timeout = TimeSpan.FromHours(options.Value.TimeoutMinutes);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MultipartCleanerBackgroundService started");

        using PeriodicTimer timer = new(_checkPeriod);

        // when stop requested, waiting for timer to tick will be cancelled too
        while (!stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var now = DateTime.UtcNow;

                var buckets = await _s3Provider.ListBucketsAsync(stoppingToken);

                var uploads = await _s3Provider.ListMultipartUploadsAsync(buckets, stoppingToken);

                var toDelete = uploads.Where(d => now - d.InitiatedAt > _timeout).ToList();
                foreach (var uploadInfo in toDelete)
                {
                    await _s3Provider.AbortMultipartUploadAsync(
                        location: new FileLocation(uploadInfo.Key, uploadInfo.BucketName),
                        uploadId: uploadInfo.UploadId,
                        ct: stoppingToken);
                }

                _logger.LogInformation("MultipartCleanerBackgroundService executed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        _logger.LogInformation("MultipartCleanerBackgroundService finished");
    }
}

public class MultipartCleanerOptions
{
    public const string SECTION_NAME = "MultipartCleaner";

    public required float CheckPeriodMinutes { get; set; }
    public required float TimeoutMinutes { get; set; }
}
