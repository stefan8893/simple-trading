using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.Domain.DataAccess;

public interface IProfileRepository : IRepository<Profile>
{
    Task<IEnumerable<Profile>> GetAll();
    Task<IEnumerable<Profile>> Find(string likeName);
}