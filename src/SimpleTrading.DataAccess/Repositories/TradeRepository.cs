using JetBrains.Annotations;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.DataAccess.Repositories;

[UsedImplicitly]
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

    public Task<Asset?> FindAsset(Guid assetId)
    {
        return dbContext.Assets
            .FindAsync(assetId)
            .AsTask();
    }

    public Task<Profile?> FindProfile(Guid profileId)
    {
        return dbContext.Profiles
            .FindAsync(profileId)
            .AsTask();
    }

    public Task<Currency?> FindCurrency(Guid currencyId)
    {
        return dbContext.Currencies
            .FindAsync(currencyId)
            .AsTask();
    }
}