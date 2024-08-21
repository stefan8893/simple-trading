using System.Linq.Expressions;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.Sorting;

public class SortByOpened(Order order) : SortBase<Trade>(PropertyFilter.Opened, order)
{
    public override Expression<Func<Trade, object?>> Selector => t => t.Opened;
}

public class SortByClosed(Order order) : SortBase<Trade>(PropertyFilter.Opened, order)
{
    public override Expression<Func<Trade, object?>> Selector => t => t.Closed;
}

public class SortByBalance(Order order)  : SortBase<Trade>(PropertyFilter.Opened, order)
{
    public override Expression<Func<Trade, object?>> Selector => t => t.Balance;
}

public class SortBySize(Order order) : SortBase<Trade>(PropertyFilter.Opened, order)
{
    public override Expression<Func<Trade, object?>> Selector => t => t.Size;
}

public class SortByResult(Order order)  : SortBase<Trade>(PropertyFilter.Opened, order)
{
    public override Expression<Func<Trade, object?>> Selector => t => t.Result == null ? -1 : t.Result.Index;
}