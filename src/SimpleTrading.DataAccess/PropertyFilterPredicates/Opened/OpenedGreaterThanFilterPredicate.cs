using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.Opened;

public class OpenedGreaterThanFilterPredicate(IValueParser<DateTimeOffset> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset>(PropertyFilter.Opened, PropertyFilter.Operator.GreaterThan,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset value)
    {
        return t => t.Opened > value.UtcDateTime;
    }
}