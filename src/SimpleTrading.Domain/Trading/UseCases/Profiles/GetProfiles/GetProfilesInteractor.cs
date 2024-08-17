using FluentValidation;
using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.Profiles.GetProfiles;

public class GetProfilesInteractor(IValidator<GetProfilesRequestModel> validator, IProfileRepository profileRepository)
    : BaseInteractor, IGetProfiles
{
    public async Task<OneOf<IReadOnlyList<GetProfilesResponseModel>, BadInput>> Execute(GetProfilesRequestModel model)
    {
        var validation = await validator.ValidateAsync(model);
        if (!validation.IsValid)
            return BadInput(validation);

        var useSearchTerm = !string.IsNullOrWhiteSpace(model.SearchTerm);

        var result = useSearchTerm
            ? await profileRepository.Find(model.SearchTerm!)
            : await profileRepository.GetAll();

        return result
            .Select(x => new GetProfilesResponseModel(x.Id, x.Name, x.Description, x.IsSelected))
            .ToList();
    }
}