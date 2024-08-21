using FluentValidation;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;

public class SortModelValidator : AbstractValidator<SortModel>
{
    public SortModelValidator(IReadOnlyDictionary<string, Func<Order, ISort<Trade>>> sorterByName)
    {
        RuleFor(x => x.Property)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(sorterByName.ContainsKey)
            .WithMessage(SimpleTradingStrings.SortingNotSupported)
            .WithName(SimpleTradingStrings.Sort);
    }
}