using DirectoryProject.FileService.Contracts.Dto;

namespace DirectoryProject.FileService.WebAPI.FileManagement;

public interface IS3Provider
{
    Task<FileMetadata> GetFileMetadataAsync(
        FileLocation location,
        CancellationToken ct = default);
    Task<string> StartMultipartUploadAsync(
        string fileName,
        string contentType,
        FileLocation location,
        CancellationToken ct = default);
    Task AbortMultipartUploadAsync(
        FileLocation location,
        string uploadId,
        CancellationToken ct = default);
    Task<string> CompleteMultipartUploadAsync(
        FileLocation location,
        string uploadId,
        List<PartETagModel> partETags,
        CancellationToken ct = default);
    Task<IReadOnlyList<string>> GenerateAllChunkUploadUrlsAsync(
        FileLocation location,
        string uploadId,
        int totalChunks);
    Task<string> GenerateChunkUploadUrlAsync(
        FileLocation location,
        string uploadId,
        int partNumber);
    
    Task<string> DeleteFileAsync(
        FileLocation location,
        CancellationToken ct = default);
    Task<string> GenerateUploadUrlAsync(
        string fileName,
        FileLocation location,
        CancellationToken ct = default);
    Task<List<FileURL>> GenerateDownloadUrlsAsync(
        List<FileLocation> locations);
    Task<FileURL> GenerateSingleDownloadUrlAsync(
        FileLocation location);

    Task<List<string>> ListBucketsAsync(CancellationToken ct = default);
    Task<List<(string BucketName, string UploadId, string Key, DateTime InitiatedAt)>> ListMultipartUploadsAsync(
        IEnumerable<string> bucketNames,
        CancellationToken ct = default);

}
