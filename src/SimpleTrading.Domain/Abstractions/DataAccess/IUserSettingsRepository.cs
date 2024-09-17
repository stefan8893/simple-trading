using SimpleTrading.Domain.User;

namespace SimpleTrading.Domain.Abstractions.DataAccess;

public interface IUserSettingsRepository
{
    ValueTask<UserSettings> GetUserSettings();
    public ValueTask<UserSettings?> GetUserSettingsOrDefault();
}