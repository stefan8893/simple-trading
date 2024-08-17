using SimpleTrading.Domain.User;

namespace SimpleTrading.Domain.Abstractions.DataAccess;

public interface IUserSettingsRepository
{
    ValueTask<UserSettings> Get();
    public ValueTask<UserSettings?> GetOrDefault();
}