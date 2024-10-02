using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.Trading.DataAccess;

public interface IProfileRepository : IRepository<Profile>
{
    ValueTask<IEnumerable<Profile>> GetAll();
    ValueTask<IEnumerable<Profile>> Find(string likeName);
}