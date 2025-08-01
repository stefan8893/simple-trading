using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.Profiles.GetProfiles;

using GetProfilesResponse = OneOf<IReadOnlyList<ProfileResponseModel>, BadInput>;

[UsedImplicitly]
public class GetProfilesInteractor(IProfileRepository profileRepository)
    : InteractorBase, IInteractor<GetProfilesRequestModel, GetProfilesResponse>
{
    public async Task<GetProfilesResponse> Execute(GetProfilesRequestModel model)
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