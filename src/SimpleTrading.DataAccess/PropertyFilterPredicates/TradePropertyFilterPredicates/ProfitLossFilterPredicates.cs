using System.Linq.Expressions;
using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure.Filter;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.TradePropertyFilterPredicates;

[UsedImplicitly]
public class ProfitLossEqualToFilterPredicate(IValueParser<decimal?> valueParser)
    : FilterPredicateBase<Trade, decimal?>(TradeProperty.ProfitLoss, Operator.EqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal? value)
    {
        return t => t.ProfitLoss == value;
    }
}

[UsedImplicitly]
public class ProfitLossGreaterThanFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(TradeProperty.ProfitLoss, Operator.GreaterThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.ProfitLoss != null && t.ProfitLoss.Value > value;
    }
}

[UsedImplicitly]
public class ProfitLossGreaterThanOrEqualToFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(TradeProperty.ProfitLoss, Operator.GreaterThanOrEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.ProfitLoss != null && t.ProfitLoss.Value >= value;
    }
}

[UsedImplicitly]
public class ProfitLossLessThanFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(TradeProperty.ProfitLoss, Operator.LessThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.ProfitLoss != null && t.ProfitLoss.Value < value;
    }
}

[UsedImplicitly]
public class ProfitLossLessThanOrEqualToFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(TradeProperty.ProfitLoss, Operator.LessThanOrEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.ProfitLoss != null && t.ProfitLoss.Value <= value;
    }
}

[UsedImplicitly]
public class ProfitLossNotEqualToFilterPredicate(IValueParser<decimal?> valueParser)
    : FilterPredicateBase<Trade, decimal?>(TradeProperty.ProfitLoss, Operator.NotEqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal? value)
    {
        return t => t.ProfitLoss != value;
    }
}