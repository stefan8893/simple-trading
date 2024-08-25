using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ResultIndex;

public class ResultLessThanFilterPredicate(IValueParser<Result> valueParser)
    : FilterPredicateBase<Trade, Result>(PropertyFilter.Result, PropertyFilter.Operator.LessThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(Result value)
    {
        var index = value.Index;

        return t => t.Result != null && t.Result.Index < index;
    }
}