using System.Linq.Expressions;
using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.Infrastructure.DataAccess;

public interface ISort<TEntity> where TEntity : IEntity
{
    Order Order { get; }

    bool IsAscendingOrder => Order == Order.Ascending;

    Expression<Func<TEntity, object?>> Selector { get; }
}