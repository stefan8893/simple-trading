using System.Linq.Expressions;
using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure.Filter;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.TradePropertyFilterPredicates;

[UsedImplicitly]
public class ClosedEqualToFilterPredicate(IValueParser<DateTimeOffset?> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset?>(TradeProperty.Closed, Operator.EqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset? value)
    {
        var nullableValue = value?.UtcDateTime;
        return t => t.Closed == nullableValue;
    }
}

[UsedImplicitly]
public class ClosedGreaterThanFilterPredicate(IValueParser<DateTimeOffset> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset>(TradeProperty.Closed, Operator.GreaterThan,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset value)
    {
        return t => t.Closed > value.UtcDateTime;
    }
}

[UsedImplicitly]
public class ClosedGreaterThanOrEqualToFilterPredicate(IValueParser<DateTimeOffset> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset>(TradeProperty.Closed, Operator.GreaterThanOrEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset value)
    {
        return t => t.Closed >= value.UtcDateTime;
    }
}

[UsedImplicitly]
public class ClosedLessThanFilterPredicate(IValueParser<DateTimeOffset> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset>(TradeProperty.Closed, Operator.LessThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset value)
    {
        return t => t.Closed < value.UtcDateTime;
    }
}

[UsedImplicitly]
public class ClosedLessThanOrEqualToFilterPredicate(IValueParser<DateTimeOffset> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset>(TradeProperty.Closed, Operator.LessThanOrEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset value)
    {
        return t => t.Closed <= value.UtcDateTime;
    }
}

[UsedImplicitly]
public class ClosedNotEqualToFilterPredicate(IValueParser<DateTimeOffset?> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset?>(TradeProperty.Closed, Operator.NotEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset? value)
    {
        var nullableValue = value?.UtcDateTime;
        return t => t.Closed != nullableValue;
    }
}