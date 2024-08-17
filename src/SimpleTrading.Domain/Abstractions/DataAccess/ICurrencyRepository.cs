using SimpleTrading.Domain.Trading;

namespace SimpleTrading.Domain.Abstractions.DataAccess;

public interface ICurrencyRepository : IRepository<Currency>
{
    Task<IEnumerable<Currency>> GetAll();
    Task<IEnumerable<Currency>> Find(string likeName);
}