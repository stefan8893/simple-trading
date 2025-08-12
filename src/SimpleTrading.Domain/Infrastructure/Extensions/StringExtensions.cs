namespace SimpleTrading.Domain.Infrastructure.Extensions;

public static class StringExtensions
{
    public static bool IsNullLiteral(this string candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate))
            return false;

        return candidate
            .Trim()
            .Equals("null", StringComparison.OrdinalIgnoreCase);
    }

    public static string FirstCharToLower(this string s)
    {
        if (string.IsNullOrEmpty(s) || char.IsLower(s[0]))
            return s;

        return char.ToLower(s[0]) + s[1..];
    }
}