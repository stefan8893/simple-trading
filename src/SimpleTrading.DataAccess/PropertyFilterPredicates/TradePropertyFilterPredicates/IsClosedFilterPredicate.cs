using System.Linq.Expressions;
using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure.Filter;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.TradePropertyFilterPredicates;

[UsedImplicitly]
public class IsClosedFilterPredicate(IValueParser<bool> valueParser)
    : FilterPredicateBase<Trade, bool>(TradeProperty.IsClosed, Operator.EqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(bool value)
    {
        return value 
            ? t => t.Closed != null && t.ProfitLoss != null 
            : t => t.Closed == null && t.ProfitLoss == null;
    }
}