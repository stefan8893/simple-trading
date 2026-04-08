namespace SimpleTrading.Domain.Infrastructure.Extensions;

public static class StringExtensions
{
    extension(string candidate)
    {
        public bool IsNullLiteral()
        {
            if (string.IsNullOrWhiteSpace(candidate))
                return false;

            return candidate
                .Trim()
                .Equals("null", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsBoolLiteral()
        {
            if (string.IsNullOrWhiteSpace(candidate))
                return false;

            var trimmedCandidate = candidate.Trim();

            return trimmedCandidate.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                   trimmedCandidate.Equals("false", StringComparison.OrdinalIgnoreCase);
        }

        public string FirstCharToLower()
        {
            if (string.IsNullOrEmpty(candidate) || char.IsLower(candidate[0]))
                return candidate;

            return char.ToLower(candidate[0]) + candidate[1..];
        }
    }
}