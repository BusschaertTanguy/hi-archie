using System.Text.RegularExpressions;

namespace Common.Infrastructure.Extensions;

public static partial class StringExtensions
{
    public static string ToKebabCase(this string? value, string separator = ".")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return string.Join(
            "-",
            value.Split(separator, StringSplitOptions.RemoveEmptyEntries).SelectMany(v => CapitalSplitRegex().Split(v)).Select(v => v.ToLowerInvariant())
        );
    }

    [GeneratedRegex("(?<!^)(?=[A-Z])")]
    private static partial Regex CapitalSplitRegex();
}