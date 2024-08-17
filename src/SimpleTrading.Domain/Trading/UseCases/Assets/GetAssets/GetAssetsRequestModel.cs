using FluentValidation;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.Assets.GetAssets;

public record GetAssetsRequestModel(string? SearchTerm);

public class GetAssetsRequestModelValidator : AbstractValidator<GetAssetsRequestModel>
{
    public GetAssetsRequestModelValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(50)
            .WithName(SimpleTradingStrings.SearchTerm);
    }
}