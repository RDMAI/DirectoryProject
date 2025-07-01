namespace DirectoryProject.FileService.WebAPI.FileManagement;

public record S3Options(
    string Endpoint,
    string AccessKey,
    string SecretKey,
    bool WithSSL,
    double URLExpirationDays)
{
    public const string S3_SECTION = "S3";
}
