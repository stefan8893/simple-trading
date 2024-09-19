using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.DataAccess.Repositories;

public class TradeRepository(TradingDbContext dbContext) : RepositoryBase<Trade>(dbContext), ITradeRepository
{
    public void AddReference(Reference reference)
    {
        dbContext.References.Add(reference);
    }

    public void RemoveReferences(IEnumerable<Reference> references)
    {
        dbContext.References.RemoveRange(references);
    }

    public ValueTask<Asset?> FindAsset(Guid assetId)
    {
        return dbContext.Assets.FindAsync(assetId);
    }

    public ValueTask<Profile?> FindProfile(Guid profileId)
    {
        return dbContext.Profiles.FindAsync(profileId);
    }

    public ValueTask<Currency?> FindCurrency(Guid currencyId)
    {
        return dbContext.Currencies.FindAsync(currencyId);
    }
}