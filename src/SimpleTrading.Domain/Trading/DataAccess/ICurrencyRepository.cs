using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.Trading.DataAccess;

public interface ICurrencyRepository : IRepository<Currency>
{
    ValueTask<IEnumerable<Currency>> GetAll();
    ValueTask<IEnumerable<Currency>> Find(string likeName);
}