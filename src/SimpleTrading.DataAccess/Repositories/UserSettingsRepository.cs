using JetBrains.Annotations;
using SimpleTrading.Domain;
using SimpleTrading.Domain.User;
using SimpleTrading.Domain.User.DataAccess;

namespace SimpleTrading.DataAccess.Repositories;

[UsedImplicitly]
public class UserSettingsRepository(TradingDbContext dbContext) : IUserSettingsRepository
{
    public async Task<UserSettings> GetUserSettings()
    {
        var userSettings = await GetUserSettingsOrDefault();

        return userSettings ??
               throw new Exception("UserSettings couldn't be loaded from the database. This should not gonna happen.");
    }

    public Task<UserSettings?> GetUserSettingsOrDefault()
    {
        return dbContext.UserSettings
            .FindAsync(Constants.UserSettingsId)
            .AsTask();
    }
}