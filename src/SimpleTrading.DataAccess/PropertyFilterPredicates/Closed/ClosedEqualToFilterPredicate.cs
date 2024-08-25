using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.Closed;

public class ClosedEqualToFilterPredicate(IValueParser<DateTimeOffset?> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset?>(PropertyFilter.Closed, PropertyFilter.Operator.EqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset? value)
    {
        var nullableValue = value?.UtcDateTime;
        return t => t.Closed == nullableValue;
    }
}