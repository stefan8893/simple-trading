using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.DataAccess.Repositories;

public class RepositoryBase<T>(DbContext dbContext) : IRepository<T> where T : class, IEntity
{
    public async ValueTask<T> Get(Guid id)
    {
        var entity = await dbContext.FindAsync<T>(id);

        return entity ?? throw new Exception($"{typeof(T).Name} not found.");
    }

    public ValueTask<T?> Find(Guid id)
    {
        return dbContext.FindAsync<T>(id);
    }

    public async Task<IReadOnlyList<T>> Find(Expression<Func<T, bool>> predicate, IEnumerable<ISort<T>>? sorting = null)
    {
        return await FindInternal(predicate, sorting)
            .ToListAsync();
    }

    public async Task<PagedList<T>> Find(PaginationConfiguration pagination, Expression<Func<T, bool>> filterPredicate,
        IEnumerable<ISort<T>>? sorting = null)
    {
        var page = pagination.Page;
        var pageSize = pagination.PageSize;
        var sortingAsList = sorting.AsList();

        var count = await FindInternal(filterPredicate, sortingAsList).CountAsync();

        if (count == 0)
            return new PagedList<T>([], 0, page, pageSize);

        var result = await FindInternal(filterPredicate, sortingAsList)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedList<T>(result, count, page, pageSize);
    }

    public void Add(T entity)
    {
        dbContext.Add(entity);
    }

    public void Remove(T entity)
    {
        dbContext.Remove(entity);
    }

    public void RemoveMany(IEnumerable<T> entities)
    {
        dbContext.RemoveRange(entities);
    }

    private IQueryable<T> FindInternal(Expression<Func<T, bool>> filterPredicate, IEnumerable<ISort<T>>? sorting = null)
    {
        var filtered = dbContext
            .Set<T>()
            .Where(filterPredicate);

        var sortingAsList = sorting.AsList();
        if (sortingAsList.Count == 0)
            return filtered;

        var firstSorting = sortingAsList.First();

        var first = firstSorting.IsAscendingOrder
            ? filtered.OrderBy(firstSorting.Selector)
            : filtered.OrderByDescending(firstSorting.Selector);

        return sortingAsList
            .Skip(1)
            .Aggregate(first, (acc, next)
                => next.IsAscendingOrder
                    ? acc.ThenBy(next.Selector)
                    : acc.ThenByDescending(next.Selector));
    }
}