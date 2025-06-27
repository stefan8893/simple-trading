using System.Globalization;
using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.User.DataAccess;

namespace SimpleTrading.Domain.User.UseCases.GetUserSettings;

[UsedImplicitly]
public class GetUserSettingsInteractor(IUserSettingsRepository userSettingsRepository)
    : InteractorBase, IInteractor<UserSettingsResponseModel>
{
    public async Task<UserSettingsResponseModel> Execute()
    {
        var userSettings = await userSettingsRepository.GetUserSettings();
        var language = userSettings.Language;

        return new UserSettingsResponseModel(userSettings.Culture, language, userSettings.TimeZone,
            userSettings.LastModified.ToLocal(userSettings.TimeZone));
    }
}