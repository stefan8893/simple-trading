using System.Linq.Expressions;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

public class SizeFilter : IPropertyFilter<Trade, decimal>
{
    public string PropertyName => SupportedPropertyFilters.Size;
    public required string Operator { get; init; }
    public required decimal ComparisonValue { get; init; }

    public static bool CanParseComparisonValue(string candidate)
    {
        return decimal.TryParse(candidate, out _);
    }

    public Expression<Func<Trade, bool>> GetPredicate(IPropertyFilterComparisonVisitor<Trade> visitor)
    {
        return visitor.Visit(this);
    }

    public static IPropertyFilter<Trade, decimal> Create(string @operator, string comparisonValue)
    {
        return new SizeFilter
        {
            Operator = @operator,
            ComparisonValue = decimal.Parse(comparisonValue)
        };
    }
}