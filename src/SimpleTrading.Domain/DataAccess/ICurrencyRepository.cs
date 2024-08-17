using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.Domain.DataAccess;

public interface ICurrencyRepository : IRepository<Currency>
{
    Task<IEnumerable<Currency>> GetAll();
    Task<IEnumerable<Currency>> Find(string likeName);
}