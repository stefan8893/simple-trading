using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.TradePropertyFilterPredicates;

public class BalanceEqualToFilterPredicate(IValueParser<decimal?> valueParser)
    : FilterPredicateBase<Trade, decimal?>(PropertyFilter.Balance, PropertyFilter.Operator.EqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal? value)
    {
        return t => t.Balance == value;
    }
}

public class BalanceGreaterThanFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(PropertyFilter.Balance, PropertyFilter.Operator.GreaterThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Balance != null && t.Balance.Value > value;
    }
}

public class BalanceGreaterThanOrEqualToFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(PropertyFilter.Balance, PropertyFilter.Operator.GreaterThanOrEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Balance != null && t.Balance.Value >= value;
    }
}

public class BalanceLessThanFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(PropertyFilter.Balance, PropertyFilter.Operator.LessThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Balance != null && t.Balance.Value < value;
    }
}

public class BalanceLessThanOrEqualToFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(PropertyFilter.Balance, PropertyFilter.Operator.LessThanOrEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Balance != null && t.Balance.Value <= value;
    }
}

public class BalanceNotEqualToFilterPredicate(IValueParser<decimal?> valueParser)
    : FilterPredicateBase<Trade, decimal?>(PropertyFilter.Balance, PropertyFilter.Operator.NotEqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal? value)
    {
        return t => t.Balance != value;
    }
}