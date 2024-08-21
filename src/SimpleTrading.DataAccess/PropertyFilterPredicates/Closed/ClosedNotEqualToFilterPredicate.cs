using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.Closed;

public class ClosedNotEqualToFilterPredicate(IValueParser<DateTimeOffset?> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset?>(PropertyFilter.Closed, PropertyFilter.Operator.NotEqualTo,
        valueParser)
{
    public override Expression<Func<Trade, bool>> GetPredicate(string comparisonValue, bool isLiteral)
    {
        var value = ValueParser.Parse(comparisonValue, isLiteral)?.UtcDateTime;

        return t => t.Closed != value;
    }
}