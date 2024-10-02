using System.Globalization;
using JetBrains.Annotations;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.User.DataAccess;

namespace SimpleTrading.Domain.User.UseCases.GetUserSettings;

[UsedImplicitly]
public class GetUserSettingsInteractor(IUserSettingsRepository userSettingsRepository)
    : InteractorBase, IInteractor<UserSettingsResponseModel>
{
    public async Task<UserSettingsResponseModel> Execute()
    {
        var userSettings = await userSettingsRepository.GetUserSettings();
        var language = userSettings.Language ?? new CultureInfo(userSettings.Culture).TwoLetterISOLanguageName;

        return new UserSettingsResponseModel(userSettings.Culture, language, userSettings.TimeZone,
            userSettings.LastModified.ToLocal(userSettings.TimeZone));
    }
}