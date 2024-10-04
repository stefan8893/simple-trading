using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.Currencies.GetCurrencies;

public record GetCurrenciesRequestModel(string? SearchTerm);

[UsedImplicitly]
public class GetCurrenciesRequestModelValidator : AbstractValidator<GetCurrenciesRequestModel>
{
    public GetCurrenciesRequestModelValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(50)
            .WithName(SimpleTradingStrings.SearchTerm);
    }
}