using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.User.DataAccess;

namespace SimpleTrading.Domain.User.UseCases.UpdateUserSettings;

[UsedImplicitly]
public class UpdateUserSettingsInteractor(IUserSettingsRepository userSettingsRepository, UowCommit uowCommit)
    : InteractorBase, IInteractor<UpdateUserSettingsRequestModel, OneOf<Completed, BadInput>>
{
    public async Task<OneOf<Completed, BadInput>> Execute(UpdateUserSettingsRequestModel requestModel)
    {
        var userSettings = await userSettingsRepository.GetUserSettings();
        
        if(requestModel.Culture is not null)
            userSettings.Culture = requestModel.Culture;
        
        if(requestModel.IsoLanguageCode.IsT0)
            userSettings.Language = requestModel.IsoLanguageCode.AsT0;
        
        if(requestModel.Timezone is not null)
            userSettings.TimeZone = requestModel.Timezone;
        
        await uowCommit();

        return Completed();
    }
}