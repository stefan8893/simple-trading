using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.DataAccess;

namespace SimpleTrading.Domain.Trading.DataAccess;

public interface ITradeRepository : IRepository<Trade>
{
    void AddReference(Reference reference);
    void RemoveReferences(IEnumerable<Reference> references);

    Task<Asset?> FindAsset(Guid assetId);
    Task<Profile?> FindProfile(Guid profileId);
    Task<Currency?> FindCurrency(Guid currencyId);
}