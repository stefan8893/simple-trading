using SimpleTrading.Domain.Trading;

namespace SimpleTrading.Domain.Abstractions.DataAccess;

public interface IAssetRepository : IRepository<Asset>
{
    Task<IEnumerable<Asset>> GetAll();
    Task<IEnumerable<Asset>> Find(string likeName);
}