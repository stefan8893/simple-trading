namespace SimpleTrading.Domain.Infrastructure.Extensions;

public static class StringExtensions
{
    extension(string value)
    {
        public bool IsNullLiteral()
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return value
                .Trim()
                .Equals("null", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsBoolLiteral()
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmedValue = value.Trim();

            return trimmedValue.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                   trimmedValue.Equals("false", StringComparison.OrdinalIgnoreCase);
        }

        public string FirstCharToLower()
        {
            if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
                return value;

            return char.ToLower(value[0]) + value[1..];
        }
    }
}
