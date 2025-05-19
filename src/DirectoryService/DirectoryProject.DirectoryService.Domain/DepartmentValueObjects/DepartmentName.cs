using DirectoryProject.DirectoryService.Domain.Shared;

namespace DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;

public class DepartmentName
{
    public const int MAX_LENGTH = 150;
    public const int MIN_LENGTH = 3;

    public string Value { get; }

    public static Result<DepartmentName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(DepartmentName));

        if (value.Length < MIN_LENGTH || value.Length > MAX_LENGTH)
            return ErrorHelper.General.ValueIsInvalid(nameof(DepartmentName));

        return new DepartmentName(value);
    }

    private DepartmentName(string value)
    {
        Value = value;
    }

    // EF Core
    private DepartmentName() { }
}
