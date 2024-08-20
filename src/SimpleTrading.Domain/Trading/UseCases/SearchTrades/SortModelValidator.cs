using FluentValidation;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertySorting;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades;

public class SortModelValidator : AbstractValidator<SortModel>
{
    public SortModelValidator()
    {
        RuleFor(x => x.Property)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(x => PropertySortingFactory.SupportedSortingProperties.Contains(x))
            .WithMessage(SimpleTradingStrings.SortingNotSupported)
            .WithName(SimpleTradingStrings.Sort);
    }
}