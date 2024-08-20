using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

namespace SimpleTrading.DataAccess.PropertyFilterVisitors;

public class GreaterThanOrEqualToPropertyFilterVisitor : IPropertyFilterComparisonVisitor<Trade>
{
    public Expression<Func<Trade, bool>> Visit(OpenedFilter openedFilter)
    {
        return t => t.Opened >= openedFilter.ComparisonValue.UtcDateTime;
    }

    public Expression<Func<Trade, bool>> Visit(BalanceFilter balanceFilter)
    {
        if (!balanceFilter.ComparisonValue.HasValue)
            return t => false;

        return t => t.Balance >= balanceFilter.ComparisonValue;
    }

    public Expression<Func<Trade, bool>> Visit(ClosedFilter closedFilter)
    {
        if (!closedFilter.ComparisonValue.HasValue)
            return t => false;

        return t => t.Closed.HasValue && t.Closed.Value >= closedFilter.ComparisonValue.Value.UtcDateTime;
    }

    public Expression<Func<Trade, bool>> Visit(SizeFilter sizeFilter)
    {
        return t => t.Size >= sizeFilter.ComparisonValue;
    }

    public Expression<Func<Trade, bool>> Visit(ResultFilter resultFilter)
    {
        var isComparisonValueNull = resultFilter.ComparisonValue is null;

        if (isComparisonValueNull)
            return t => false;

        var index = Result.GetIndexOf(resultFilter.ComparisonValue!);

        return t => (t.Result == null && isComparisonValueNull) ||
                    (t.Result != null && !isComparisonValueNull &&
                     t.Result.Index >= index);
    }
}