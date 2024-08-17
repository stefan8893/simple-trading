using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Abstractions.DataAccess;

namespace SimpleTrading.DataAccess.Repositories;

public class RepositoryBase<T>(DbContext dbContext) : IRepository<T> where T : class
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

    public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
    {
        return await dbContext.Set<T>()
            .Where(predicate)
            .ToListAsync();
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
}