using SimpleTrading.Domain.Infrastructure.DataAccess;

namespace SimpleTrading.Domain.Trading.DataAccess;

public interface ITradeRepository : IRepository<Trade>
{
    void AddReference(Reference reference);
    void RemoveReferences(IEnumerable<Reference> references);
}