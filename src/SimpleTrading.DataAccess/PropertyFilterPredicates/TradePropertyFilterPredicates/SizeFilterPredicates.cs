using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.TradePropertyFilterPredicates;

public class SizeEqualToFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(PropertyFilter.Size, PropertyFilter.Operator.EqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Size == value;
    }
}

public class SizeGreaterThanFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(PropertyFilter.Size, PropertyFilter.Operator.GreaterThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Size > value;
    }
}

public class SizeGreaterThanOrEqualToFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(PropertyFilter.Size, PropertyFilter.Operator.GreaterThanOrEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Size >= value;
    }
}

public class SizeLessThanFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(PropertyFilter.Size, PropertyFilter.Operator.LessThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Size < value;
    }
}

public class SizeLessThanOrEqualToFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(PropertyFilter.Size, PropertyFilter.Operator.LessThanOrEqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Size <= value;
    }
}

public class SizeNotEqualToFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(PropertyFilter.Size, PropertyFilter.Operator.NotEqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Size != value;
    }
}