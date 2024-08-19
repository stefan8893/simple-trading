using System.Linq.Expressions;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Abstractions.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertySorting;

public record SortByOpened(Order Order) : ISort<Trade>
{
    public Expression<Func<Trade, object?>> Selector => t => t.Opened;
}

public record SortByClosed(Order Order) : ISort<Trade>
{
    public Expression<Func<Trade, object?>> Selector => t => t.Closed;
}

public record SortByBalance(Order Order) : ISort<Trade>
{
    public Expression<Func<Trade, object?>> Selector => t => t.Balance;
}

public record SortBySize(Order Order) : ISort<Trade>
{
    public Expression<Func<Trade, object?>> Selector => t => t.Size;
}

public record SortByResult(Order Order) : ISort<Trade>
{
    public Expression<Func<Trade, object?>> Selector => t => t.Result == null ? -1 : t.Result.Index;
}