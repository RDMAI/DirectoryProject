namespace DirectoryProject.FileService.WebAPI.FileManagement;

public class S3Options
{
    public const string S3_SECTION = "S3";

    public string Endpoint { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public bool WithSSL { get; set; }
    public double URLExpirationDays { get; set; }
}
