using System.Text;
using System.Text.RegularExpressions;

namespace DirectoryProject.DirectoryService.Application.Shared.Services;

public class SlugService
{
    private readonly Settings _settings;

    public SlugService(Settings settings)
    {
        _settings = settings;
    }

    public string ConvertStringToSlug(string input)
    {
        // replace all white spaces with -
        input = new Regex(@"\s+").Replace(input.ToLower(), "-");

        // replace all characters, specified in settings
        var sb = new StringBuilder();
        foreach (var ch in input.ToLower())
        {
            if (_settings.ReplaceChars.TryGetValue(ch, out var replacement))
                sb.Append(replacement);
            else
                sb.Append(ch);
        }
        input = sb.ToString();

        // remove all characters, except lower case latin, digits, '.' and '-'
        input = new Regex(@"[^a-z0-9\-\.]").Replace(input, string.Empty);

        return sb.ToString();
    }

    public class Settings
    {
        public Dictionary<char, string> ReplaceChars { get; set; } = [];
    }
}
