using System.Linq.Expressions;
using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure.Filter;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.TradePropertyFilterPredicates;

[UsedImplicitly]
public class OpenedEqualToFilterPredicate(IValueParser<DateTimeOffset> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset>(TradeProperty.Opened, Operator.EqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset value)
    {
        return t => t.Opened == value.UtcDateTime;
    }
}

[UsedImplicitly]
public class OpenedGreaterThanFilterPredicate(IValueParser<DateTimeOffset> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset>(TradeProperty.Opened, Operator.GreaterThan,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset value)
    {
        return t => t.Opened > value.UtcDateTime;
    }
}

[UsedImplicitly]
public class OpenedGreaterThanOrEqualToFilterPredicate(IValueParser<DateTimeOffset> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset>(TradeProperty.Opened, Operator.GreaterThanOrEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset value)
    {
        return t => t.Opened >= value.UtcDateTime;
    }
}

[UsedImplicitly]
public class OpenedLessThanFilterPredicate(IValueParser<DateTimeOffset> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset>(TradeProperty.Opened, Operator.LessThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset value)
    {
        return t => t.Opened < value.UtcDateTime;
    }
}

[UsedImplicitly]
public class OpenedLessThanOrEqualToFilterPredicate(IValueParser<DateTimeOffset> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset>(TradeProperty.Opened, Operator.LessThanOrEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset value)
    {
        return t => t.Opened <= value.UtcDateTime;
    }
}

[UsedImplicitly]
public class OpenedNotEqualToFilterPredicate(IValueParser<DateTimeOffset> valueParser)
    : FilterPredicateBase<Trade, DateTimeOffset>(TradeProperty.Opened, Operator.NotEqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(DateTimeOffset value)
    {
        return t => t.Opened != value.UtcDateTime;
    }
}