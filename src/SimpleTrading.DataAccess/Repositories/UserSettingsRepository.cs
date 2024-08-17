using SimpleTrading.Domain;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.User;

namespace SimpleTrading.DataAccess.Repositories;

public class UserSettingsRepository(TradingDbContext dbContext) : IUserSettingsRepository
{
    public async ValueTask<UserSettings> Get()
    {
        var userSettings = await GetOrDefault();

        return userSettings ??
               throw new Exception("UserSettings couldn't be loaded from the database. This should not gonna happen.");
    }

    public ValueTask<UserSettings?> GetOrDefault()
    {
        return dbContext.UserSettings.FindAsync(Constants.UserSettingsId);
    }
}