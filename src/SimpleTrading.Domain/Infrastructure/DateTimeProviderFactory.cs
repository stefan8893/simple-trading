using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.Domain.Infrastructure;

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