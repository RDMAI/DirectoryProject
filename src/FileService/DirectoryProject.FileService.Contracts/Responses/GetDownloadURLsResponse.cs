using DirectoryProject.FileService.Contracts.Dto;

namespace DirectoryProject.FileService.Contracts.Responses;

public record GetDownloadURLsResponse(
        List<FileURL> URLs);
