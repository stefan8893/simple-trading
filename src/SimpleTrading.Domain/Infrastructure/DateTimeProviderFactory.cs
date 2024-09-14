using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.Domain.Infrastructure;

public static class DateTimeProviderFactory
{
    public static UtcNow UtcNow()
    {
        return () => DateTime.UtcNow;
    }

    public static LocalNow LocalNow(IUserSettingsRepository userSettingsRepository)
    {
        return async () =>
        {
            var userSettings = await userSettingsRepository.Get();

            return DateTime.UtcNow
                .ToLocal(userSettings.TimeZone).DateTime
                .ToLocalKind();
        };
    }
}