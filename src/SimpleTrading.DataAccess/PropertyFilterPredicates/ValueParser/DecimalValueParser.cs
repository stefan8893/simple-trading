using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ValueParser;

public class DecimalValueParser : IValueParser<decimal>
{
    public bool CanParse(string candidate, bool isLiteral)
    {
        return decimal.TryParse(candidate, out _);
    }

    public decimal Parse(string candidate, bool isLiteral)
    {
        return decimal.Parse(candidate);
    }
}

public class NullableDecimalValueParser : IValueParser<decimal?>
{
    public bool CanParse(string candidate, bool isLiteral)
    {
        if (isLiteral && candidate.IsNullLiteral())
            return true;

        return decimal.TryParse(candidate, out _);
    }

    public decimal? Parse(string candidate, bool isLiteral)
    {
        return isLiteral && candidate.IsNullLiteral()
            ? null
            : decimal.Parse(candidate);
    }
}