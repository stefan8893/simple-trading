using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.User.DataAccess;

namespace SimpleTrading.Domain.User.Factories;

public static class DateTimeProviderFactory
{
    public static LocalNow CreateLocalNowFunc(IUserSettingsRepository userSettingsRepository)
    {
        return async () =>
        {
            var userSettings = await userSettingsRepository.GetUserSettings();

            return DateTime.UtcNow.ToLocal(userSettings.TimeZone);
        };
    }
}