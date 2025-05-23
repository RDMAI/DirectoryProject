using DirectoryProject.DirectoryService.Domain.Shared;

namespace DirectoryProject.DirectoryService.Domain.PositionValueObjects;

public class PositionName
{
    public const int MAX_LENGTH = 100;
    public const int MIN_LENGTH = 3;

    public string Value { get; }

    public static Result<PositionName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(PositionName));

        if (MIN_LENGTH > value.Length || value.Length > MAX_LENGTH)
            return ErrorHelper.General.ValueIsInvalid(nameof(PositionName));

        return new PositionName(value);
    }

    private PositionName(string value)
    {
        Value = value;
    }

    // EF Core
    private PositionName() { }
}
