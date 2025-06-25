using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.User.DataAccess;

namespace SimpleTrading.Domain.User.UseCases.UpdateUserLanguage;

[UsedImplicitly]
public class UpdateUserLanguageInteractor(IUserSettingsRepository userSettingsRepository, UowCommit uowCommit)
    : InteractorBase, IInteractor<UpdateUserLanguageRequestModel, OneOf<Completed, BadInput>>
{
    public async Task<OneOf<Completed, BadInput>> Execute(UpdateUserLanguageRequestModel requestModel)
    {
        var userSettings = await userSettingsRepository.GetUserSettings();
        userSettings.Language = requestModel.IsoLanguageCode;
        await uowCommit();

        return Completed();
    }
}