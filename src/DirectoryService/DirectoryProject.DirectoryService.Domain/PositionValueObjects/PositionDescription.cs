using SharedKernel;

namespace DirectoryProject.DirectoryService.Domain.PositionValueObjects;

public class PositionDescription
{
    public const int MAX_LENGTH = 1000;

    public string Value { get; }

    public static Result<PositionDescription> Create(string value)
    {
        if (value.Length > MAX_LENGTH)
            return ErrorHelper.General.ValueIsInvalid(nameof(PositionDescription));

        return new PositionDescription(value);
    }

    private PositionDescription(string value)
    {
        Value = value;
    }

    // EF Core
    private PositionDescription() { }
}
