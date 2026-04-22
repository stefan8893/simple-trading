using System.Linq.Expressions;
using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure.Filter;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.TradePropertyFilterPredicates;

[UsedImplicitly]
public class IsFinishedFilterPredicate(IValueParser<bool> valueParser)
    : FilterPredicateBase<Trade, bool>(TradeProperty.IsFinished, Operator.EqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(bool value)
    {
        return value
            ? t => t.Finished != null && t.ProfitLoss != null
            : t => t.Finished == null && t.ProfitLoss == null;
    }
}