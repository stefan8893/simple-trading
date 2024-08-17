using System.Linq.Expressions;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

public class ResultFilter : IPropertyFilter<Trade, string>
{
    public string PropertyName => SupportedPropertyFilters.Result;
    public required string Operator { get; init; }
    public required string ComparisonValue { get; init; }

    public static bool CanParseComparisonValue(string candidate)
    {
        return !string.IsNullOrWhiteSpace(candidate) && Result.GetIndexOf(candidate) >= 0;
    }

    public Expression<Func<Trade, bool>> GetPredicate(IPropertyFilterComparisonVisitor<Trade> visitor)
    {
        return visitor.Visit(this);
    }

    public static IPropertyFilter<Trade, string> Create(string @operator, string comparisonValue)
    {
        return new ResultFilter
        {
            Operator = @operator,
            ComparisonValue = comparisonValue
        };
    }
}