using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ValueParser;

public class DateTimeOffsetValueParser : IValueParser<DateTimeOffset>
{
    public bool TryParse(string candidate, bool isLiteral, out DateTimeOffset result)
    {
        return DateTimeOffset.TryParse(candidate, out result);
    }
}

public class NullableDateTimeOffsetValueParser : IValueParser<DateTimeOffset?>
{
    public bool TryParse(string candidate, bool isLiteral, out DateTimeOffset? result)
    {
        result = null;

        if (isLiteral && candidate.IsNullLiteral())
            return true;

        result = DateTimeOffset.Parse(candidate);
        return true;
    }
}