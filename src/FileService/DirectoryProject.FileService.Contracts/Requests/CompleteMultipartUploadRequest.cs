using DirectoryProject.FileService.Contracts.Dto;

namespace DirectoryProject.FileService.Contracts.Requests;

public record CompleteMultipartUploadRequest(
        FileLocation Location,
        string UploadId,
        List<PartETagModel> PartETags);
