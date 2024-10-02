using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.Trading.DataAccess;

public interface IAssetRepository : IRepository<Asset>
{
    ValueTask<IEnumerable<Asset>> GetAll();
    ValueTask<IEnumerable<Asset>> Find(string likeName);
}