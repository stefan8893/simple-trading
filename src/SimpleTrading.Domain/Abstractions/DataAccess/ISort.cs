using System.Linq.Expressions;

namespace SimpleTrading.Domain.Abstractions.DataAccess;

public interface ISort<TEntity> where TEntity : IEntity
{
    string PropertyName { get; }   
    Order Order { get; }

    bool IsAscendingOrder => Order == Order.Ascending;
    bool IsDescendingOrder => !IsAscendingOrder;

    Expression<Func<TEntity, object?>> Selector { get; }
}