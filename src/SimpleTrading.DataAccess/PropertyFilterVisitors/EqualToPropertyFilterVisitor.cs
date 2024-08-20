using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

namespace SimpleTrading.DataAccess.PropertyFilterVisitors;

public class EqualToPropertyFilterVisitor : IPropertyFilterComparisonVisitor<Trade>
{
    public Expression<Func<Trade, bool>> Visit(OpenedFilter openedFilter)
    {
        return t => t.Opened == openedFilter.ComparisonValue.UtcDateTime;
    }

    public Expression<Func<Trade, bool>> Visit(BalanceFilter balanceFilter)
    {
        return t => t.Balance == balanceFilter.ComparisonValue;
    }

    public Expression<Func<Trade, bool>> Visit(ClosedFilter closedFilter)
    {
        var comparisonValue = closedFilter.ComparisonValue?.UtcDateTime;

        return t => t.Closed == comparisonValue;
    }

    public Expression<Func<Trade, bool>> Visit(SizeFilter sizeFilter)
    {
        return t => t.Size == sizeFilter.ComparisonValue;
    }

    public Expression<Func<Trade, bool>> Visit(ResultFilter resultFilter)
    {
        var isComparisonValueNull = resultFilter.ComparisonValue is null;

        var index = Result.GetIndexOf(resultFilter.ComparisonValue ?? "");

        return t => (t.Result == null && isComparisonValueNull) ||
                    (t.Result != null && !isComparisonValueNull &&
                     t.Result.Index == index);
    }
}