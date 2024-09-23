using System.Linq.Expressions;
using SimpleTrading.DataAccess.PropertyFilterPredicates.ValueParser.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.TradePropertyFilterPredicates;

public class ResultEqualToFilterPredicate(IValueParser<NullableReference<Result>> valueParser)
    : FilterPredicateBase<Trade, NullableReference<Result>>(PropertyFilter.Result, PropertyFilter.Operator.EqualTo,
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

public class ResultGreaterThanFilterPredicate(IValueParser<Result> valueParser)
    : FilterPredicateBase<Trade, Result>(PropertyFilter.Result, PropertyFilter.Operator.GreaterThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(Result value)
    {
        var index = value.Index;

        return t => t.Result != null && t.Result.Index > index;
    }
}

public class ResultGreaterThanOrEqualToFilterPredicate(IValueParser<Result> valueParser)
    : FilterPredicateBase<Trade, Result>(PropertyFilter.Result, PropertyFilter.Operator.GreaterThanOrEqualTo,
        valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(Result value)
    {
        var index = value.Index;

        return t => t.Result != null && t.Result.Index >= index;
    }
}

public class ResultLessThanFilterPredicate(IValueParser<Result> valueParser)
    : FilterPredicateBase<Trade, Result>(PropertyFilter.Result, PropertyFilter.Operator.LessThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(Result value)
    {
        var index = value.Index;

        return t => t.Result != null && t.Result.Index < index;
    }
}

public class ResultLessThanOrEqualToFilterPredicate(IValueParser<Result> valueParser)
    : FilterPredicateBase<Trade, Result>(PropertyFilter.Result, PropertyFilter.Operator.LessThanOrEqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(Result value)
    {
        var index = value.Index;

        return t => t.Result != null && t.Result.Index <= index;
    }
}

public class ResultNotEqualToFilterPredicate(IValueParser<NullableReference<Result>> valueParser)
    : FilterPredicateBase<Trade, NullableReference<Result>>(PropertyFilter.Result, PropertyFilter.Operator.NotEqualTo,
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