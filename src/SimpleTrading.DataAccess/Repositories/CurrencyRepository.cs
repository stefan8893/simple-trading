using Microsoft.EntityFrameworkCore;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.DataAccess.Repositories;

public class CurrencyRepository(TradingDbContext dbContext) : RepositoryBase<Currency>(dbContext), ICurrencyRepository
{
    public async ValueTask<IEnumerable<Currency>> GetAll()
    {
        return await dbContext.Currencies.ToListAsync();
    }

    public async ValueTask<IEnumerable<Currency>> Find(string likeName)
    {
        var nameLowered = likeName.ToLower();

        return await dbContext.Currencies
            .Where(x => EF.Functions.Like(x.Name.ToLower(), $"%{nameLowered}%"))
            .ToListAsync();
    }
}