using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ValueParser;

public class DateTimeOffsetValueParser : IValueParser<DateTimeOffset>
{
    public bool CanParse(string candidate, bool isLiteral)
    {
        return DateTimeOffset.TryParse(candidate, out _);
    }

    public DateTimeOffset Parse(string candidate, bool isLiteral)
    {
        return DateTimeOffset.Parse(candidate);
    }
}

public class NullableDateTimeOffsetValueParser : IValueParser<DateTimeOffset?>
{
    public bool CanParse(string candidate, bool isLiteral)
    {
        if (isLiteral && candidate.IsNullLiteral())
            return true;

        return DateTimeOffset.TryParse(candidate, out _);
    }

    public DateTimeOffset? Parse(string candidate, bool isLiteral)
    {
        if (isLiteral && candidate.IsNullLiteral())
            return null;

        return DateTimeOffset.Parse(candidate);
    }
}