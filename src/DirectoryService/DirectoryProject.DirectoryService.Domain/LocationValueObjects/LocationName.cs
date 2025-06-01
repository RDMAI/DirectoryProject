using DirectoryProject.DirectoryService.Domain.Shared;

namespace DirectoryProject.DirectoryService.Domain.LocationValueObjects;

public class LocationName
{
    public const int MAX_LENGTH = 120;
    public const int MIN_LENGTH = 3;

    public string Value { get; }

    public static explicit operator string(LocationName locationName) =>
        locationName.Value;

    public static Result<LocationName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(LocationName));

        if (value.Length < MIN_LENGTH || value.Length > MAX_LENGTH)
            return ErrorHelper.General.ValueIsInvalid(nameof(LocationName));

        return new LocationName(value);
    }

    private LocationName(string value)
    {
        Value = value;
    }

    // EF Core
    private LocationName() { }
}
