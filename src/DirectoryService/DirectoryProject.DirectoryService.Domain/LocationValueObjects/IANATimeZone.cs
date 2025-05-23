using DirectoryProject.DirectoryService.Domain.Shared;
using NodaTime;

namespace DirectoryProject.DirectoryService.Domain.LocationValueObjects;

public class IANATimeZone
{
    public string Value { get; }

    public static Result<IANATimeZone> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(IANATimeZone));

        if (DateTimeZoneProviders.Tzdb.GetZoneOrNull(value) is null)
            return ErrorHelper.General.ValueIsInvalid(nameof(IANATimeZone));

        return new IANATimeZone(value);
    }

    private IANATimeZone(string value)
    {
        Value = value;
    }

    // EF Core
    private IANATimeZone() { }
}
