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

    public async Task<Asset> GetAsset(Guid assetId)
    {
        var asset = await FindAsset(assetId);
        return asset ?? throw new Exception($"{nameof(Asset)} not found.");
    }

    public Task<Profile?> FindProfile(Guid profileId)
    {
        return dbContext.Profiles
            .FindAsync(profileId)
            .AsTask();
    }

    public async Task<Profile> GetProfile(Guid profileId)
    {
        var profile = await FindProfile(profileId);
        return profile ?? throw new Exception($"{nameof(Profile)} not found.");
    }

    public Task<Currency?> FindCurrency(Guid currencyId)
    {
        return dbContext.Currencies
            .FindAsync(currencyId)
            .AsTask();
    }

    public async Task<Currency> GetCurrency(Guid currencyId)
    {
        var currency = await FindCurrency(currencyId);
        return currency ?? throw new Exception($"{nameof(Currency)} not found.");
    }
}