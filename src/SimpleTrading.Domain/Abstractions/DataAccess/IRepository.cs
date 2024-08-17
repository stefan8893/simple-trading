using System.Linq.Expressions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Abstractions.DataAccess;

public interface IRepository<T>
{
    ValueTask<T> Get(Guid id);
    ValueTask<T?> Find(Guid id);
    Task<IReadOnlyList<T>> Find(Expression<Func<T, bool>> predicate, IEnumerable<ISort<T>>? sorting = null);

    Task<PagedList<T>> Find(PaginationConfiguration pagination, Expression<Func<T, bool>> predicate,
        IEnumerable<ISort<T>>? sorting = null);

    void Add(T entity);
    void Remove(T entity);
    void RemoveMany(IEnumerable<T> entities);
}