using System.Linq.Expressions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.DataAccess;

namespace SimpleTrading.Domain.Abstractions;

public interface IRepository<TEntity> where TEntity: IEntity
{
    ValueTask<TEntity> Get(Guid id);
    ValueTask<TEntity?> Find(Guid id);
    Task<IReadOnlyList<TEntity>> Find(Expression<Func<TEntity, bool>> filterPredicate, IEnumerable<ISort<TEntity>>? sorting = null);

    Task<PagedList<TEntity>> Find(PaginationConfiguration pagination, Expression<Func<TEntity, bool>> filterPredicate,
        IEnumerable<ISort<TEntity>>? sorting = null);

    void Add(TEntity entity);
    void Remove(TEntity entity);
    void RemoveMany(IEnumerable<TEntity> entities);
}