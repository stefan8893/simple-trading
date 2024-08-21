using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ResultIndex;

public class ResultEqualToFilterPredicate(IValueParser<Result?> valueParser)
    : FilterPredicateBase<Trade, Result?>(PropertyFilter.Result, PropertyFilter.Operator.EqualTo, valueParser)
{
    public override Expression<Func<Trade, bool>> GetPredicate(string comparisonValue, bool isLiteral)
    {
        var value = ValueParser.Parse(comparisonValue, isLiteral);

        var isResultNull = value is null;
        var index = value?.Index;

        return t => (t.Result == null && isResultNull) ||
                    (t.Result != null && !isResultNull &&
                     t.Result.Index == index);
    }
}