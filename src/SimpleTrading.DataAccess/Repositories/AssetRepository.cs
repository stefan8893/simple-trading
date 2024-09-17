using Microsoft.EntityFrameworkCore;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.DataAccess.Repositories;

public class AssetRepository(TradingDbContext dbContext) : RepositoryBase<Asset>(dbContext), IAssetRepository
{
    public async Task<IEnumerable<Asset>> GetAll()
    {
        return await dbContext.Assets.ToListAsync();
    }

    public async Task<IEnumerable<Asset>> Find(string likeName)
    {
        var nameLowered = likeName.ToLower();

        return await dbContext.Assets
            .Where(x => EF.Functions.Like(x.Name.ToLower(), $"%{nameLowered}%"))
            .ToListAsync();
    }
}