using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ResultIndex;

public class ResultNotEqualToFilterPredicate(IValueParser<Result?> valueParser)
    : FilterPredicateBase<Trade, Result?>(PropertyFilter.Result, PropertyFilter.Operator.NotEqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(Result? value)
    {
        var isResultNull = value is null;
        var index = value?.Index;

        return t => (isResultNull && t.Result != null) ||
                    (t.Result != null && !isResultNull && t.Result.Index != index);
    }
}