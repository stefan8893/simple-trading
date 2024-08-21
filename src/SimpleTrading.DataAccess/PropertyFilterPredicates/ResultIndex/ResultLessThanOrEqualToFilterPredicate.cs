using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ResultIndex;

public class ResultLessThanOrEqualToFilterPredicate(IValueParser<Result> valueParser)
    : FilterPredicateBase<Trade, Result>(PropertyFilter.Result, PropertyFilter.Operator.LessThanOrEqualTo, valueParser)
{
    public override Expression<Func<Trade, bool>> GetPredicate(string comparisonValue, bool isLiteral)
    {
        var value = ValueParser.Parse(comparisonValue, isLiteral);
        var index = value.Index;

        return t => t.Result != null && t.Result.Index <= index;
    }
}