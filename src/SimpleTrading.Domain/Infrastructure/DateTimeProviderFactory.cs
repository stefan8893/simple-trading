namespace SimpleTrading.Domain.Infrastructure;

using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Extensions;

public static class DateTimeProviderFactory
{
    public static UtcNow UtcNow() => () => DateTime.UtcNow;

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