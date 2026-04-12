using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure.Extensions;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ValueParser;

[UsedImplicitly]
public class DecimalValueParser : IValueParser<decimal>
{
    public bool TryParse(string candidate, bool isLiteral, out decimal result)
    {
        return decimal.TryParse(candidate, out result);
    }
}

[UsedImplicitly]
public class NullableDecimalValueParser : IValueParser<decimal?>
{
    public bool TryParse(string candidate, bool isLiteral, out decimal? result)
    {
        result = null;

        if (isLiteral)
            return candidate.IsNullLiteral();

        var isParsable = decimal.TryParse(candidate, out var parsed);
        result = isParsable ? parsed : null;
        return isParsable;
    }
}