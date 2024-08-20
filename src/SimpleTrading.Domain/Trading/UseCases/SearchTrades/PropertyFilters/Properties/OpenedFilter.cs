using System.Linq.Expressions;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

public class OpenedFilter : IPropertyFilter<Trade, DateTimeOffset>
{
    public string PropertyName => PropertyFilter.Opened;
    public required string Operator { get; init; }
    public required DateTimeOffset ComparisonValue { get; init; }

    public Expression<Func<Trade, bool>> GetPredicate(IPropertyFilterComparisonVisitor<Trade> visitor)
    {
        return visitor.Visit(this);
    }

    public static bool TryParseComparisonValue(string candidate, out DateTimeOffset result)
    {
        return DateTimeOffset.TryParse(candidate, out result);
    }

    public static IPropertyFilter<Trade, DateTimeOffset> Create(string @operator, string comparisonValue,
        bool isLiteral)
    {
        return new OpenedFilter
        {
            Operator = @operator,
            ComparisonValue = DateTimeOffset.Parse(comparisonValue)
        };
    }
}