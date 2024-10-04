using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.DataAccess.Repositories;

[UsedImplicitly]
public class ProfileRepository(TradingDbContext dbContext) : RepositoryBase<Profile>(dbContext), IProfileRepository
{
    public async Task<IEnumerable<Profile>> GetAll()
    {
        return await dbContext.Profiles.ToListAsync();
    }

    public async Task<IEnumerable<Profile>> Find(string likeName)
    {
        var nameLowered = likeName.ToLower();

        return await dbContext.Profiles
            .Where(x => EF.Functions.Like(x.Name.ToLower(), $"%{nameLowered}%"))
            .ToListAsync();
    }
}