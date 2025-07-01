using DirectoryProject.FileService.WebAPI.Domain;

namespace DirectoryProject.FileService.WebAPI.FileManagement;

public interface IS3Provider
{
    Task<string> StartMultipartUpload(
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
    Task<IReadOnlyList<string>> GenerateAllChunkUploadUrls(
        FileLocation location,
        string uploadId,
        int totalChunks);
    Task<string> GenerateChunkUploadUrl(
        FileLocation location,
        string uploadId,
        int partNumber);
    
    Task<string> DeleteFileAsync(
        FileLocation location,
        CancellationToken ct = default);
    Task<string> GenerateUploadUrl(
        string fileName,
        FileLocation location,
        CancellationToken ct = default);
    Task<string> GenerateDownloadUrlAsync(
        FileLocation location,
        int expirationHours);

    Task<List<string>> ListBucketsAsync(CancellationToken ct = default);
}
