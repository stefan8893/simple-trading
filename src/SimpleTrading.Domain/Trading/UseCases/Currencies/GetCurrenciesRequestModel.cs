using FluentValidation;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.Currencies;

public record GetCurrenciesRequestModel(string? SearchTerm);

public class GetCurrenciesRequestModelValidator : AbstractValidator<GetCurrenciesRequestModel>
{
    public GetCurrenciesRequestModelValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(50)
            .WithName(SimpleTradingStrings.SearchTerm);
    }
}