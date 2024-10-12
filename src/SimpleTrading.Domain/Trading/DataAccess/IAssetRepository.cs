using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.DataAccess;

namespace SimpleTrading.Domain.Trading.DataAccess;

public interface IAssetRepository : IRepository<Asset>
{
    Task<IEnumerable<Asset>> GetAll();
    Task<IEnumerable<Asset>> Find(string likeName);
}