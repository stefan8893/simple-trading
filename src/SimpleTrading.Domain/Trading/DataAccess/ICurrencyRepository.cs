using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.DataAccess;

namespace SimpleTrading.Domain.Trading.DataAccess;

public interface ICurrencyRepository : IRepository<Currency>
{
    Task<IEnumerable<Currency>> GetAll();
    Task<IEnumerable<Currency>> Find(string likeName);
}