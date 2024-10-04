using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.Profiles.GetProfiles;

public record GetProfilesRequestModel(string? SearchTerm);

[UsedImplicitly]
public class GetProfilesRequestModelValidator : AbstractValidator<GetProfilesRequestModel>
{
    public GetProfilesRequestModelValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(50)
            .WithName(SimpleTradingStrings.SearchTerm);
    }
}