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

        if (isLiteral && candidate.IsNullLiteral())
            return true;

        result = decimal.Parse(candidate);
        return true;
    }
}