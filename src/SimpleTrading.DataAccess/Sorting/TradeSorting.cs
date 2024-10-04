using System.Linq.Expressions;
using SimpleTrading.Domain.Infrastructure.DataAccess;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.Sorting;

public class SortByOpened(Order order) : SortBase<Trade>(order)
{
    public override Expression<Func<Trade, object?>> Selector => t => t.Opened;
}

public class SortByClosed(Order order) : SortBase<Trade>(order)
{
    public override Expression<Func<Trade, object?>> Selector => t => t.Closed;
}

public class SortByBalance(Order order) : SortBase<Trade>(order)
{
    public override Expression<Func<Trade, object?>> Selector => t => t.Balance;
}

public class SortBySize(Order order) : SortBase<Trade>(order)
{
    public override Expression<Func<Trade, object?>> Selector => t => t.Size;
}

public class SortByResult(Order order) : SortBase<Trade>(order)
{
    public override Expression<Func<Trade, object?>> Selector => t => t.Result == null ? -1 : t.Result.Index;
}