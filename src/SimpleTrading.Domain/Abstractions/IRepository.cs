﻿using System.Linq.Expressions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.DataAccess;

namespace SimpleTrading.Domain.Abstractions;

public interface IRepository<TEntity> where TEntity : IEntity
{
    Task<TEntity> Get(Guid id);
    Task<TEntity?> Find(Guid id);

    Task<IReadOnlyList<TEntity>> Find(Expression<Func<TEntity, bool>> filterPredicate,
        IEnumerable<ISort<TEntity>>? sorting = null);

    Task<PagedList<TEntity>> Find(Expression<Func<TEntity, bool>> filterPredicate, PaginationConfiguration pagination,
        IEnumerable<ISort<TEntity>>? sorting = null);

    void Add(TEntity entity);
    void Remove(TEntity entity);
    void RemoveMany(IEnumerable<TEntity> entities);
}