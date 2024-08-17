using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.Domain.DataAccess;

public interface IAssetRepository : IRepository<Asset>
{
    Task<IEnumerable<Asset>> GetAll();
    Task<IEnumerable<Asset>> Find(string likeName);
}