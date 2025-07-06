using SharedKernel;

namespace DirectoryProject.FileService.Communication;

public class FileErrorHelper
{
    public static Error DeletionFailed(string? mes = null)
    {
        return Error.Failure(
            "file.deletion.failed",
            mes ?? "Failed to delete file");
    }

    public static Error UploadFailed(string? mes = null)
    {
        return Error.Failure(
            "file.upload.failed",
            mes ?? "Failed to upload");
    }

    public static Error DownloadFailed(string? mes = null)
    {
        return Error.Failure(
            "file.download.failed",
            mes ?? "Failed to download file");
    }
}
