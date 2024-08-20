using System.Linq.Expressions;
using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

public class ResultFilter : IPropertyFilter<Trade, string?>
{
    public string PropertyName => PropertyFilter.Result;
    public required string Operator { get; init; }
    public required string? ComparisonValue { get; init; }

    public Expression<Func<Trade, bool>> GetPredicate(IPropertyFilterComparisonVisitor<Trade> visitor)
    {
        return visitor.Visit(this);
    }

    public static bool TryParseValue(string candidate, bool isLiteral, out string? result)
    {
        result = null;

        if (isLiteral && candidate.IsNullLiteral())
            return true;

        var isSuccess = Result.GetIndexOf(candidate) >= 0;
        if (!isSuccess)
            return false;

        result = new Result(candidate, TradingResultSource.ManuallyEntered).Name;
        return true;
    }

    public static IPropertyFilter<Trade, string?> Create(string @operator, string comparisonValue, bool isLiteral)
    {
        var isSuccess = TryParseValue(comparisonValue, isLiteral, out var value);

        if (!isSuccess)
            throw new Exception($"The comparison value '{comparisonValue}' couldn't be parsed.");

        return new ResultFilter
        {
            Operator = @operator,
            ComparisonValue = value
        };
    }
}