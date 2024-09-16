using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.TradePropertyFilterPredicates;

public class ResultEqualToFilterPredicate(IValueParser<Result?> valueParser)
    : FilterPredicateBase<Trade, Result?>(PropertyFilter.Result, PropertyFilter.Operator.EqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(Result? value)
    {
        var isResultNull = value is null;
        var index = value?.Index;

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

public class ResultNotEqualToFilterPredicate(IValueParser<Result?> valueParser)
    : FilterPredicateBase<Trade, Result?>(PropertyFilter.Result, PropertyFilter.Operator.NotEqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(Result? value)
    {
        var isResultNull = value is null;
        var index = value?.Index;

        return t => (isResultNull && t.Result != null) ||
                    (t.Result != null && !isResultNull && t.Result.Index != index);
    }
}