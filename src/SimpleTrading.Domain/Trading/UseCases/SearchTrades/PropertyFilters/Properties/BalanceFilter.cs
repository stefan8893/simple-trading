using System.Linq.Expressions;
using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

public class BalanceFilter : IPropertyFilter<Trade, decimal?>
{
    public string PropertyName => PropertyFilter.Balance;
    public required string Operator { get; init; }
    public required decimal? ComparisonValue { get; init; }

    public Expression<Func<Trade, bool>> GetPredicate(IPropertyFilterComparisonVisitor<Trade> visitor)
    {
        return visitor.Visit(this);
    }

    public static bool TryParseComparisonValue(string candidate, bool isLiteral, out decimal? result)
    {
        result = null;

        if (isLiteral && candidate.IsNullLiteral())
            return true;

        if (!decimal.TryParse(candidate, out var parsed))
            return false;

        result = parsed;
        return true;
    }

    public static IPropertyFilter<Trade, decimal?> Create(string @operator, string comparisonValue, bool isLiteral)
    {
        var isSuccess = TryParseComparisonValue(comparisonValue, isLiteral, out var value);

        if (!isSuccess)
            throw new Exception($"The comparison value '{comparisonValue}' couldn't be parsed.");

        return new BalanceFilter
        {
            Operator = @operator,
            ComparisonValue = value
        };
    }
}