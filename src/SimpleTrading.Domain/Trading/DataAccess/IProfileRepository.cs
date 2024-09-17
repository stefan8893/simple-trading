namespace SimpleTrading.Domain.Trading.DataAccess;

public interface IProfileRepository : IRepository<Profile>
{
    Task<IEnumerable<Profile>> GetAll();
    Task<IEnumerable<Profile>> Find(string likeName);
}