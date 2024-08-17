using System.Linq.Expressions;

namespace SimpleTrading.Domain.Abstractions.DataAccess;

public interface IRepository<T>
{
    ValueTask<T> Get(Guid id);
    ValueTask<T?> Find(Guid id);
    Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);

    void Add(T entity);
    void Remove(T entity);
    void RemoveMany(IEnumerable<T> entities);
}