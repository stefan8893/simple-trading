using FluentValidation;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.Profiles.GetProfiles;

public class GetProfilesInteractor(IValidator<GetProfilesRequestModel> validator, IProfileRepository profileRepository)
    : BaseInteractor, IGetProfiles
{
    public async Task<OneOf<IReadOnlyList<GetProfilesResponseModel>, BadInput>> Execute(GetProfilesRequestModel model)
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