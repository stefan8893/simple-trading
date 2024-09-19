using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.Trading.DataAccess;

public interface IAssetRepository : IRepository<Asset>
{
    Task<IEnumerable<Asset>> GetAll();
    Task<IEnumerable<Asset>> Find(string likeName);
}