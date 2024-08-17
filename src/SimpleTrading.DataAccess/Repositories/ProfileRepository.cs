﻿using Microsoft.EntityFrameworkCore;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.Repositories;

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