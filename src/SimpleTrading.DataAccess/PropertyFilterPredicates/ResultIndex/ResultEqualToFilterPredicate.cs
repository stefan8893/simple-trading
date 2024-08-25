using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ResultIndex;

public class ResultEqualToFilterPredicate(IValueParser<Result?> valueParser)
    : FilterPredicateBase<Trade, Result?>(PropertyFilter.Result, PropertyFilter.Operator.EqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(Result? value)
    {
        var isResultNull = value is null;
        var index = value?.Index;

        return t => (t.Result == null && isResultNull) ||
                    (t.Result != null && !isResultNull &&
                     t.Result.Index == index);
    }
}