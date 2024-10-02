using FluentValidation;
using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.Profiles.GetProfiles;

using GetProfilesResponse = OneOf<IReadOnlyList<GetProfilesResponseModel>, BadInput>;

[UsedImplicitly]
public class GetProfilesInteractor(IValidator<GetProfilesRequestModel> validator, IProfileRepository profileRepository)
    : InteractorBase, IInteractor<GetProfilesRequestModel, GetProfilesResponse>
{
    public async Task<GetProfilesResponse> Execute(GetProfilesRequestModel model)
    {
        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
            return BadInput(validationResult);

        var useSearchTerm = !string.IsNullOrWhiteSpace(model.SearchTerm);

        var result = useSearchTerm
            ? await profileRepository.Find(model.SearchTerm!)
            : await profileRepository.GetAll();

        return result
            .Select(x => new GetProfilesResponseModel(x.Id, x.Name, x.Description, x.IsSelected))
            .ToList();
    }
}