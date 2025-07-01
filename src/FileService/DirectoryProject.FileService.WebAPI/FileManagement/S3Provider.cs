using Amazon.S3;
using Amazon.S3.Model;
using DirectoryProject.FileService.WebAPI.Domain;

namespace DirectoryProject.FileService.WebAPI.FileManagement;

public class S3Provider : IS3Provider
{
    public static readonly string[] BucketNames = ["photos", "videos"];

    private readonly IAmazonS3 _s3;
    private readonly S3Options _options;
    private readonly ILogger<S3Provider> _logger;

    public S3Provider(
        IAmazonS3 s3,
        S3Options options,
        ILogger<S3Provider> logger)
    {
        _s3 = s3;
        _options = options;
        _logger = logger;
    }

    public async Task<string> StartMultipartUpload(
        string fileName,
        string contentType,
        FileLocation location,
        CancellationToken ct = default)
    {
        var request = new InitiateMultipartUploadRequest
        {
            BucketName = location.BucketName,
            Key = location.FileId,
            ContentType = contentType,
            Metadata =
            {
                ["file-name"] = fileName,
            },
        };

        var response = await _s3.InitiateMultipartUploadAsync(request, ct);

        _logger.LogInformation("Multipart upload started. Id {uploadId}", response.UploadId);

        return response.UploadId;
    }

    public async Task AbortMultipartUploadAsync(
        FileLocation location,
        string uploadId,
        CancellationToken ct = default)
    {
        await _s3.AbortMultipartUploadAsync(
            bucketName: location.BucketName,
            key: location.FileId,
            uploadId: uploadId,
            cancellationToken: ct);

        _logger.LogInformation("Multipart upload aborted. Id {uploadId}", uploadId);
    }

    public async Task<string> CompleteMultipartUploadAsync(
        FileLocation location,
        string uploadId,
        List<PartETagModel> partETags,
        CancellationToken ct = default)
    {
        var request = new CompleteMultipartUploadRequest
        {
            BucketName = location.BucketName,
            UploadId = uploadId,
            Key = location.FileId,
            PartETags = partETags.Select(pt => new PartETag(pt.PartNumber, pt.ETag)).ToList(),
        };

        var response = await _s3.CompleteMultipartUploadAsync(request, ct);

        _logger.LogInformation("Multipart upload completed. Id {uploadId}", uploadId);

        return response.Key;
    }

    public async Task<IReadOnlyList<string>> GenerateAllChunkUploadUrls(
        FileLocation location,
        string uploadId,
        int totalChunks)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = location.BucketName,
            Key = location.FileId,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddDays(_options.URLExpirationDays),
            UploadId = uploadId,
        };

        List<string> presignedURLs = [];
        for (int i = 1; i <= totalChunks; i++)
        {
            request.PartNumber = i;
            var response = await _s3.GetPreSignedURLAsync(request);
            presignedURLs.Add(response);
        }

        _logger.LogInformation(
            "Created multipart upload presigned URL for all chunks. File id {fileId}, upload id {uploadId}, total chunks {totalChunks}",
            location.FileId,
            uploadId,
            totalChunks);

        return presignedURLs;
    }

    public async Task<string> GenerateChunkUploadUrl(
        FileLocation location,
        string uploadId,
        int partNumber)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = location.BucketName,
            Key = location.FileId,
            Verb = HttpVerb.PUT,  // upload
            Expires = DateTime.UtcNow.AddDays(_options.URLExpirationDays),
            UploadId = uploadId,
            PartNumber = partNumber,
        };

        var response = await _s3.GetPreSignedURLAsync(request);

        _logger.LogInformation(
            "Created multipart upload presigned URL. File id {fileId}, upload id {uploadId}, part number {partNumber}",
            location.FileId,
            uploadId,
            partNumber);

        return response;
    }

    public async Task<string> DeleteFileAsync(
        FileLocation location,
        CancellationToken ct = default)
    {
        var response = await _s3.DeleteObjectAsync(
            bucketName: location.BucketName,
            key: location.FileId,
            cancellationToken: ct);

        _logger.LogInformation("File with id {fileId} was deleted", location.FileId);

        return location.FileId;
    }

    public async Task<string> GenerateUploadUrl(
        string fileName,
        FileLocation location,
        CancellationToken ct = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = location.BucketName,
            Key = location.FileId,
            Verb = HttpVerb.PUT,  // upload
            Expires = DateTime.UtcNow.AddDays(_options.URLExpirationDays),
            Metadata =
            {
                ["file-name"] = fileName,
            },
        };

        var response = await _s3.GetPreSignedURLAsync(request);

        _logger.LogInformation("Created upload presigned URL. File id {fileId}", location.FileId);

        return response;
    }

    public async Task<string> GenerateDownloadUrlAsync(
        FileLocation location,
        int expirationHours)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = location.BucketName,
            Key = location.FileId,
            Verb = HttpVerb.GET,  // download
            Expires = DateTime.UtcNow.AddHours(expirationHours),
        };

        var response = await _s3.GetPreSignedURLAsync(request);

        _logger.LogInformation("Created download presigned URL. File id {fileId}", location.FileId);

        return response;
    }

    public async Task<List<string>> ListBucketsAsync(CancellationToken ct = default)
    {
        var response = await _s3.ListBucketsAsync(ct);

        return response.Buckets.Select(d => d.BucketName).ToList();
    }
}
