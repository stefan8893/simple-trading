using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.Profiles.GetProfiles;

[UsedImplicitly]
public class GetProfilesInteractor(IProfileRepository profileRepository)
    : InteractorBase, IInteractor<GetProfilesRequestModel, IReadOnlyList<ProfileResponseModel>>
{
    public async Task<IReadOnlyList<ProfileResponseModel>> Execute(GetProfilesRequestModel model, CancellationToken cancellationToken)
    {
        var useSearchTerm = !string.IsNullOrWhiteSpace(model.SearchTerm);

        var result = useSearchTerm
            ? await profileRepository.Find(model.SearchTerm!)
            : await profileRepository.GetAll();

        return result
            .Select(ProfileResponseModel.From)
            .ToList();
    }
}