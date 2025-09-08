using System.Text;
using System.Text.RegularExpressions;

namespace MegaNotes.Api.Utilities;

public static class Slugify
{
    public static string ToSlug(string? text)
    {
        text ??= "";
        var s = text.ToLowerInvariant();
        s = Regex.Replace(s, @"[^a-z0-9\s-]", "");
        s = Regex.Replace(s, @"\s+", " ").Trim();
        s = s.Replace(" ", "-");
        return s;
    }
}
