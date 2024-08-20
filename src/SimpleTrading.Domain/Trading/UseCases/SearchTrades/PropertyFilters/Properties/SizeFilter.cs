using System.Linq.Expressions;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

public class SizeFilter : IPropertyFilter<Trade, decimal>
{
    public string PropertyName => PropertyFilter.Size;
    public required string Operator { get; init; }
    public required decimal ComparisonValue { get; init; }

    public Expression<Func<Trade, bool>> GetPredicate(IPropertyFilterComparisonVisitor<Trade> visitor)
    {
        return visitor.Visit(this);
    }

    public static bool TryParseValue(string candidate, out decimal result)
    {
        return decimal.TryParse(candidate, out result);
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