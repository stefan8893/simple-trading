namespace SimpleTrading.Domain.Trading.DataAccess;

public interface ITradeRepository : IRepository<Trade>
{
    void AddReference(Reference reference);
    void RemoveReferences(IEnumerable<Reference> references);

    ValueTask<Asset?> FindAsset(Guid assetId);
    ValueTask<Profile?> FindProfile(Guid profileId);
    ValueTask<Currency?> FindCurrency(Guid currencyId);
}