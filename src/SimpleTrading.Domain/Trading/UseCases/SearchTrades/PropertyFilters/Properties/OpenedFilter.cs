using System.Linq.Expressions;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

public class OpenedFilter : IPropertyFilter<Trade, DateTimeOffset>
{
    public string PropertyName => SupportedPropertyFilters.Opened;
    public required string Operator { get; init; }
    public required DateTimeOffset ComparisonValue { get; init; }

    public static bool CanParseComparisonValue(string candidate)
    {
        return DateTimeOffset.TryParse(candidate, out _);
    }

    public Expression<Func<Trade, bool>> GetPredicate(IPropertyFilterComparisonVisitor<Trade> visitor)
    {
        return visitor.Visit(this);
    }

    public static IPropertyFilter<Trade, DateTimeOffset> Create(string @operator, string comparisonValue)
    {
        return new OpenedFilter
        {
            Operator = @operator,
            ComparisonValue = DateTimeOffset.Parse(comparisonValue)
        };
    }
}