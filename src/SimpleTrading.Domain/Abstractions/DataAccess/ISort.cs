using System.Linq.Expressions;

namespace SimpleTrading.Domain.Abstractions.DataAccess;

public interface ISort<T>
{
    public Order Order { get; }

    public bool IsAscendingOrder => Order == Order.Ascending;
    public bool IsDescendingOrder => !IsAscendingOrder;

    public Expression<Func<T, object?>> Selector { get; }
}