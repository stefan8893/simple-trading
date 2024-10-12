using System.Linq.Expressions;
using JetBrains.Annotations;
using SimpleTrading.DataAccess.PropertyFilterPredicates.ValueParser.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Filter;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.TradePropertyFilterPredicates;

[UsedImplicitly]
public class ResultEqualToFilterPredicate(IValueParser<NullableReference<Result>> valueParser)
    : FilterPredicateBase<Trade, NullableReference<Result>>(TradeProperty.Result, Operator.EqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(NullableReference<Result> value)
    {
        var isResultNull = value.IsNull;
        var index = value.Value?.Index;

        return t => (t.Result == null && isResultNull) ||
                    (t.Result != null && !isResultNull &&
                     t.Result.Index == index);
    }
}

[UsedImplicitly]
public class ResultGreaterThanFilterPredicate(IValueParser<Result> valueParser)
    : FilterPredicateBase<Trade, Result>(TradeProperty.Result, Operator.GreaterThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(Result value)
    {
        var index = value.Index;

        return t => t.Result != null && t.Result.Index > index;
    }
}

[UsedImplicitly]
public class ResultGreaterThanOrEqualToFilterPredicate(IValueParser<Result> valueParser)
    : FilterPredicateBase<Trade, Result>(TradeProperty.Result, Operator.GreaterThanOrEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(Result value)
    {
        var index = value.Index;

        return t => t.Result != null && t.Result.Index >= index;
    }
}

[UsedImplicitly]
public class ResultLessThanFilterPredicate(IValueParser<Result> valueParser)
    : FilterPredicateBase<Trade, Result>(TradeProperty.Result, Operator.LessThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(Result value)
    {
        var index = value.Index;

        return t => t.Result != null && t.Result.Index < index;
    }
}

[UsedImplicitly]
public class ResultLessThanOrEqualToFilterPredicate(IValueParser<Result> valueParser)
    : FilterPredicateBase<Trade, Result>(TradeProperty.Result, Operator.LessThanOrEqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(Result value)
    {
        var index = value.Index;

        return t => t.Result != null && t.Result.Index <= index;
    }
}

[UsedImplicitly]
public class ResultNotEqualToFilterPredicate(IValueParser<NullableReference<Result>> valueParser)
    : FilterPredicateBase<Trade, NullableReference<Result>>(TradeProperty.Result, Operator.NotEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(NullableReference<Result> value)
    {
        var isResultNull = value.IsNull;
        var index = value.Value?.Index;

        return t => (isResultNull && t.Result != null) ||
                    (t.Result != null && !isResultNull && t.Result.Index != index);
    }
}