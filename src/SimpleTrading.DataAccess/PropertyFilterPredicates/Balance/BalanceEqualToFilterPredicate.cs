using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.Balance;

public class BalanceEqualToFilterPredicate(IValueParser<decimal?> valueParser)
    : FilterPredicateBase<Trade, decimal?>(PropertyFilter.Balance, PropertyFilter.Operator.EqualTo, valueParser)
{
    public override Expression<Func<Trade, bool>> GetPredicate(string comparisonValue, bool isLiteral)
    {
        var value = ValueParser.Parse(comparisonValue, isLiteral);

        return t => t.Balance == value;
    }
}