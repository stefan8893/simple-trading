using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.User.DataAccess;

namespace SimpleTrading.Domain.User.UseCases.GetUserSettings;

[UsedImplicitly]
public class GetUserSettingsInteractor(IUserSettingsRepository userSettingsRepository, IProfileRepository profileRepository)
    : InteractorBase, IInteractor<UserSettingsResponseModel>
{
    public async Task<UserSettingsResponseModel> Execute()
    {
        var userSettings = await userSettingsRepository.GetUserSettings();
        var language = userSettings.Language;

        var profiles = await profileRepository.Find(x => x.IsActive);
        if(profiles.Count != 1)
            throw new Exception("There can be only one active profile");
        
        var activeProfile = profiles[0];
        
        return new UserSettingsResponseModel(userSettings.Culture, language, userSettings.TimeZone,
            userSettings.LastModified.ToLocal(userSettings.TimeZone), activeProfile.Id, activeProfile.Name);
    }
}