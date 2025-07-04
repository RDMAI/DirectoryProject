using DirectoryProject.FileService.Contracts.Dto;

namespace DirectoryProject.FileService.Contracts.Responses;

public record GetUploadUrlResponse(
        FileURL URL);
