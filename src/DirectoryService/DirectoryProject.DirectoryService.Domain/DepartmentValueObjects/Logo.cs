using SharedKernel;

namespace DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;

public class Logo
{
    public const int MAX_LENGTH = 100;
    public const int MIN_LENGTH = 3;

    public string FileId { get; private set; }
    public string Location { get; private set; }
    public string FileName { get; private set; }
    public string ContentType { get; private set; }

    public static Result<Logo> Create(
        string fileId,
        string location,
        string fileName,
        string contentType)
    {
        if (string.IsNullOrEmpty(fileId))
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(FileId));

        if (string.IsNullOrWhiteSpace(fileName))
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(FileName));

        if (fileName.Length < MIN_LENGTH || fileName.Length > MAX_LENGTH)
            return ErrorHelper.General.ValueIsInvalid(nameof(FileName));

        if (string.IsNullOrWhiteSpace(contentType))
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(ContentType));

        if (contentType.Length < MIN_LENGTH || contentType.Length > MAX_LENGTH)
            return ErrorHelper.General.ValueIsInvalid(nameof(ContentType));

        return new Logo(
            fileId,
            location,
            fileName,
            contentType);
    }

    private Logo(
        string fileId,
        string location,
        string fileName,
        string contentType)
    {
        FileId = fileId;
        Location = location;
        FileName = fileName;
        ContentType = contentType;
    }

    // EF Core
    private Logo() { }
}
