using System.Linq.Expressions;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Abstractions.DataAccess;

namespace SimpleTrading.DataAccess.Sorting;

public abstract class SortBase<TEntity>(string propertyName, Order order) : ISort<TEntity>
    where TEntity : IEntity
{
    public string PropertyName { get; init; } = propertyName;
    public Order Order { get; init; } = order;
    public abstract Expression<Func<TEntity, object?>> Selector { get; }
}