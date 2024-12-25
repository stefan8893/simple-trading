using System.Linq.Expressions;
using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure.Filter;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.TradePropertyFilterPredicates;

[UsedImplicitly]
public class SizeEqualToFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(TradeProperty.Size, Operator.EqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Size == value;
    }
}

[UsedImplicitly]
public class SizeGreaterThanFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(TradeProperty.Size, Operator.GreaterThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Size > value;
    }
}

[UsedImplicitly]
public class SizeGreaterThanOrEqualToFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(TradeProperty.Size, Operator.GreaterThanOrEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Size >= value;
    }
}

[UsedImplicitly]
public class SizeLessThanFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(TradeProperty.Size, Operator.LessThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Size < value;
    }
}

[UsedImplicitly]
public class SizeLessThanOrEqualToFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(TradeProperty.Size, Operator.LessThanOrEqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Size <= value;
    }
}

[UsedImplicitly]
public class SizeNotEqualToFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(TradeProperty.Size, Operator.NotEqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Size != value;
    }
}