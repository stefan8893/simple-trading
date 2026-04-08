using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure.Extensions;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ValueParser;

[UsedImplicitly]
public class DateTimeOffsetValueParser : IValueParser<DateTimeOffset>
{
    public bool TryParse(string candidate, bool isLiteral, out DateTimeOffset result)
    {
        return DateTimeOffset.TryParse(candidate, out result);
    }
}

[UsedImplicitly]
public class NullableDateTimeOffsetValueParser : IValueParser<DateTimeOffset?>
{
    public bool TryParse(string candidate, bool isLiteral, out DateTimeOffset? result)
    {
        result = null;

        if (isLiteral)
            return candidate.IsNullLiteral();

        var isParsable = DateTimeOffset.TryParse(candidate, out var parsed);
        result = isParsable ? parsed : null;
        return isParsable;
    }
}