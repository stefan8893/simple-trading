using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.User;

namespace SimpleTrading.Domain.Extensions;

public static class TradingDbContextExtensions
{
    public static async Task<UserSettings> GetUserSettings(this TradingDbContext context)
    {
        var userSettings = await GetUserSettingsOrDefault(context);

        return userSettings ??
               throw new Exception("UserSettings couldn't be loaded from the database. This should not gonna happen.");
    }

    public static ValueTask<UserSettings?> GetUserSettingsOrDefault(this TradingDbContext context)
    {
        return context.FindAsync<UserSettings>(Constants.UserSettingsId);
    }
}