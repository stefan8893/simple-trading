using SimpleTrading.Domain;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.User;

namespace SimpleTrading.DataAccess.Repositories;

public class UserSettingsRepository(TradingDbContext dbContext) : IUserSettingsRepository
{
    public async ValueTask<UserSettings> GetUserSettings()
    {
        var userSettings = await GetUserSettingsOrDefault();

        return userSettings ??
               throw new Exception("UserSettings couldn't be loaded from the database. This should not gonna happen.");
    }

    public ValueTask<UserSettings?> GetUserSettingsOrDefault()
    {
        return dbContext.UserSettings.FindAsync(Constants.UserSettingsId);
    }
}