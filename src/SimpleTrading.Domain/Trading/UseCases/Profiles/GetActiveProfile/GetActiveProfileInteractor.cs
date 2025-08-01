using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.Profiles.GetActiveProfile;

[UsedImplicitly]
public class GetActiveProfileInteractor(IProfileRepository profileRepository)
    : IInteractor<ProfileResponseModel>
{
    public async Task<ProfileResponseModel> Execute()
    {
        var activeProfiles = await profileRepository.Find(x => x.IsActive);

        if (!activeProfiles.Any())
        {
            var allProfiles = await profileRepository.GetAll();
            return ProfileResponseModel.From(allProfiles.First());
        }

        var activeProfile = activeProfiles.Single();
        return ProfileResponseModel.From(activeProfile);
    }
}