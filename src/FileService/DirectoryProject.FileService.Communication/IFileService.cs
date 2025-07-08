using DirectoryProject.FileService.Contracts.Dto;
using DirectoryProject.FileService.Contracts.Requests;
using DirectoryProject.FileService.Contracts.Responses;
using SharedKernel;

namespace DirectoryProject.FileService.Communication;

public interface IFileService
{
    Task<Result<DeleteFileResponse>> DeleteFileAsync(
        string fileId,
        string bucketName,
        CancellationToken ct = default);

    Task<Result<GetChunkUploadURLResponse>> GetChunkUploadURLAsync(
        GetChunkUploadURLRequest request,
        CancellationToken ct = default);

    Task<Result<GetDownloadURLsResponse>> GetDownloadURLAsync(
        GetDownloadURLsRequest request,
        CancellationToken ct = default);

    Task<Result<GetUploadUrlResponse>> GetUploadURLAsync(
        GetUploadUrlRequest request,
        CancellationToken ct = default);

    Task<UnitResult> MultipartCancelUploadAsync(
        MultipartCancelUploadRequest request,
        CancellationToken ct = default);

    Task<Result<CompleteMultipartUploadResponse>> MultipartCompleteUploadAsync(
        CompleteMultipartUploadRequest request,
        CancellationToken ct = default);

    Task<Result<MultipartStartUploadResponse>> MultipartStartUploadAsync(
        MultipartStartUploadRequest request,
        CancellationToken ct = default);
}
