using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure.Filter;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;

[UsedImplicitly]
public class FilterModelValidator : AbstractValidator<FilterModel>
{
    private readonly IReadOnlyList<IFilterPredicate<Trade>> _filterPredicates;

    public FilterModelValidator(IEnumerable<IFilterPredicate<Trade>> filterPredicates)
    {
        _filterPredicates = filterPredicates.AsList();

        RuleFor(x => x.PropertyName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(x => _filterPredicates.Any(p => p.Match(x)))
            .WithMessage(SimpleTradingStrings.FilterNotSupported)
            .DependentRules(() => RuleForOperator().DependentRules(RuleForComparisonValue))
            .WithName(SimpleTradingStrings.Field);
    }

    private IRuleBuilderOptions<FilterModel, string> RuleForOperator()
    {
        return RuleFor(x => x.Operator)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must((m, x) => _filterPredicates.Any(p => p.Match(m.PropertyName, x)))
            .WithMessage(SimpleTradingStrings.OperatorNotSupported)
            .WithName(SimpleTradingStrings.Operator);
    }

    private void RuleForComparisonValue()
    {
        RuleFor(x => x.ComparisonValue)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50)
            .Must(HaveParsableComparisonValue)
            .WithMessage(x =>
                x.IsLiteral
                    ? SimpleTradingStrings.NullNotAllowed
                    : SimpleTradingStrings.ValueNotAllowed)
            .WithName(SimpleTradingStrings.ComparisonValue);
    }

    private bool HaveParsableComparisonValue(FilterModel m, string comparisonValue)
    {
        return _filterPredicates
            .First(p => p.Match(m.PropertyName, m.Operator))
            .CanParse(comparisonValue, m.IsLiteral);
    }
}