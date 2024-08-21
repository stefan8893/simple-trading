using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.Size;

public class SizeLessThanFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(PropertyFilter.Size, PropertyFilter.Operator.LessThan, valueParser)
{
    public override Expression<Func<Trade, bool>> GetPredicate(string comparisonValue, bool isLiteral)
    {
        var value = ValueParser.Parse(comparisonValue, isLiteral);

        return t => t.Size < value;
    }
}