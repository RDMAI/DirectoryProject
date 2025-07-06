using DirectoryProject.FileService.Contracts.Dto;

namespace DirectoryProject.FileService.Contracts.Requests;

public record GetDownloadURLsRequest(
        List<FileLocation> Locations);
