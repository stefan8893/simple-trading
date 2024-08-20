using System.Linq.Expressions;
using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

public class ClosedFilter : IPropertyFilter<Trade, DateTimeOffset?>
{
    public string PropertyName => PropertyFilter.Closed;
    public required string Operator { get; init; }
    public required DateTimeOffset? ComparisonValue { get; init; }

    public Expression<Func<Trade, bool>> GetPredicate(IPropertyFilterComparisonVisitor<Trade> visitor)
    {
        return visitor.Visit(this);
    }

    public static bool TryParseComparisonValue(string candidate, bool isLiteral, out DateTimeOffset? result)
    {
        result = null;

        if (isLiteral && candidate.IsNullLiteral())
            return true;

        if (!DateTimeOffset.TryParse(candidate, out var parsed))
            return false;

        result = parsed;
        return true;
    }

    public static IPropertyFilter<Trade, DateTimeOffset?> Create(string @operator, string comparisonValue,
        bool isLiteral)
    {
        var isSuccess = TryParseComparisonValue(comparisonValue, isLiteral, out var value);

        if (!isSuccess)
            throw new Exception($"The comparison value '{comparisonValue}' couldn't be parsed.");

        return new ClosedFilter
        {
            Operator = @operator,
            ComparisonValue = value
        };
    }
}