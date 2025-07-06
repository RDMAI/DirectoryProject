using System.Net.Http.Json;
using System.Threading;
using DirectoryProject.FileService.Contracts.Dto;
using DirectoryProject.FileService.Contracts.Requests;
using DirectoryProject.FileService.Contracts.Responses;
using SharedKernel;

namespace DirectoryProject.FileService.Communication;

public class FileHttpClient : IFileService
{
    private readonly HttpClient _httpClient;
    public FileHttpClient(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<DeleteFileResponse>> DeleteFileAsync(
        Guid fileId,
        string bucketName,
        CancellationToken ct = default)
    {
        var response = await _httpClient.DeleteAsync(
            requestUri:$"/api/files/{fileId}?bucketname={bucketName}",
            cancellationToken: ct);

        if (response.IsSuccessStatusCode == false)
            return FileErrorHelper.DeletionFailed();

        var deleteResponse = await response.Content.ReadFromJsonAsync<DeleteFileResponse>(ct);

        if (deleteResponse is null)
            return FileErrorHelper.DeletionFailed("Failed to parse storage response");

        return deleteResponse;
    }

    public async Task<Result<GetChunkUploadURLResponse>> GetChunkUploadURLAsync(
        GetChunkUploadURLRequest request,
        CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            requestUri: "/api/files/multipart/url",
            value: request,
            cancellationToken: ct);

        if (response.IsSuccessStatusCode == false)
            return FileErrorHelper.UploadFailed();

        var urlResponse = await response.Content.ReadFromJsonAsync<GetChunkUploadURLResponse>(ct);

        if (urlResponse is null)
            return FileErrorHelper.UploadFailed("Failed to parse storage response");

        return urlResponse;
    }

    public async Task<Result<GetDownloadURLsResponse>> GetDownloadURLAsync(
        GetDownloadURLsRequest request,
        CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            requestUri: "/api/files/downloadurls",
            value: request,
            cancellationToken: ct);

        if (response.IsSuccessStatusCode == false)
            return FileErrorHelper.DownloadFailed();

        var urlResponse = await response.Content.ReadFromJsonAsync<GetDownloadURLsResponse>(ct);

        if (urlResponse is null)
            return FileErrorHelper.DownloadFailed("Failed to parse storage response");

        return urlResponse;
    }

    public async Task<Result<GetUploadUrlResponse>> GetUploadURLAsync(
        GetUploadUrlRequest request,
        CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            requestUri: "/api/files/url",
            value: request,
            cancellationToken: ct);

        if (response.IsSuccessStatusCode == false)
            return FileErrorHelper.UploadFailed();

        var urlResponse = await response.Content.ReadFromJsonAsync<GetUploadUrlResponse>(ct);

        if (urlResponse is null)
            return FileErrorHelper.UploadFailed("Failed to parse storage response");

        return urlResponse;
    }

    public async Task<UnitResult> MultipartCancelUploadAsync(
        MultipartCancelUploadRequest request,
        CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            requestUri: "/api/files/multipart/cancel",
            value: request,
            cancellationToken: ct);

        if (response.IsSuccessStatusCode == false)
            return FileErrorHelper.UploadFailed("Failed to cancel multipart upload");

        return UnitResult.Success();
    }

    public async Task<Result<CompleteMultipartUploadResponse>> MultipartCompleteUploadAsync(
        CompleteMultipartUploadRequest request,
        CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            requestUri: "/api/files/multipart/end",
            value: request,
            cancellationToken: ct);

        if (response.IsSuccessStatusCode == false)
            return FileErrorHelper.UploadFailed("Failed to complete multipart upload");

        var urlResponse = await response.Content.ReadFromJsonAsync<CompleteMultipartUploadResponse>(ct);

        if (urlResponse is null)
            return FileErrorHelper.UploadFailed("Failed to parse storage response");

        return urlResponse;
    }

    public async Task<Result<MultipartStartUploadResponse>> MultipartStartUploadAsync(
        MultipartStartUploadRequest request,
        CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            requestUri: "/api/files/multipart/start",
            value: request,
            cancellationToken: ct);

        if (response.IsSuccessStatusCode == false)
            return FileErrorHelper.UploadFailed("Failed to start multipart upload");

        var urlResponse = await response.Content.ReadFromJsonAsync<MultipartStartUploadResponse>(ct);

        if (urlResponse is null)
            return FileErrorHelper.UploadFailed("Failed to parse storage response");

        return urlResponse;
    }
}
