using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.Assets.GetAssets;

public record GetAssetsRequestModel(string? SearchTerm);

[UsedImplicitly]
public class GetAssetsRequestModelValidator : AbstractValidator<GetAssetsRequestModel>
{
    public GetAssetsRequestModelValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(50)
            .WithName(SimpleTradingStrings.SearchTerm);
    }
}