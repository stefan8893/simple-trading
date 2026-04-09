using JetBrains.Annotations;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ValueParser;

[UsedImplicitly]
public class BooleanValueParser : IValueParser<bool>
{
    public bool TryParse(string candidate, bool isLiteral, out bool result)
    {
        result = false;
        return isLiteral && bool.TryParse(candidate, out result);
    }
}