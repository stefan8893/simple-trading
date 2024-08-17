using SimpleTrading.Domain.User;

namespace SimpleTrading.Domain.DataAccess;

public interface IUserSettingsRepository
{
    ValueTask<UserSettings> Get();
    public ValueTask<UserSettings?> GetOrDefault();
}