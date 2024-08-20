using FluentValidation;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades;

public class FilterModelValidator : AbstractValidator<FilterModel>
{
    public FilterModelValidator()
    {
        RuleFor(x => x.PropertyName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(x => PropertyFilter.All.Contains(x))
            .WithMessage(SimpleTradingStrings.FilterNotSupported)
            .DependentRules(() =>
            {
                RuleFor(x => x.ComparisonValue)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .MaximumLength(50)
                    .Must((m, x) => PropertyFilterFactory.CanCreate(m.PropertyName, x, m.IsLiteral))
                    .WithMessage(SimpleTradingStrings.ValueNotAllowed)
                    .Must(AllowNullComparison)
                    .WithMessage(SimpleTradingStrings.NullNotAllowed)
                    .WithName(SimpleTradingStrings.ComparisonValue);
            })
            .WithName(SimpleTradingStrings.Field);

        RuleFor(x => x.Operator)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(Operator.All.Contains)
            .WithMessage(SimpleTradingStrings.OperatorNotSupported)
            .WithName(SimpleTradingStrings.Operator);
    }

    private static bool AllowNullComparison(FilterModel m, string x)
    {
        return !m.IsLiteral ||
               !x.IsNullLiteral() ||
               !PropertyFilterFactory.IsNullComparisonForbidden(m.PropertyName, m.Operator);
    }
}