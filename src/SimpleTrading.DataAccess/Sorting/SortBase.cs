using System.Linq.Expressions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.DataAccess;

namespace SimpleTrading.DataAccess.Sorting;

public abstract class SortBase<TEntity>(Order order) : ISort<TEntity>
    where TEntity : IEntity
{
    public Order Order { get; } = order;
    public abstract Expression<Func<TEntity, object?>> Selector { get; }
}