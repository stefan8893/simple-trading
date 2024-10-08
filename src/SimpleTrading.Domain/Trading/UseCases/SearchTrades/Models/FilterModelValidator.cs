﻿using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;

[UsedImplicitly]
public class FilterModelValidator : AbstractValidator<FilterModel>
{
    public FilterModelValidator(IEnumerable<IFilterPredicate<Trade>> filterPredicates)
    {
        var filterPredicatesMaterialized = filterPredicates.ToList();

        RuleFor(x => x.PropertyName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(x => filterPredicatesMaterialized.Any(p => p.Match(x)))
            .WithMessage(SimpleTradingStrings.FilterNotSupported)
            .DependentRules(() =>
            {
                RuleFor(x => x.Operator)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Must((m, x) => filterPredicatesMaterialized.Any(p => p.Match(m.PropertyName, x)))
                    .WithMessage(SimpleTradingStrings.OperatorNotSupported)
                    .WithName(SimpleTradingStrings.Operator)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.ComparisonValue)
                            .Cascade(CascadeMode.Stop)
                            .NotEmpty()
                            .MaximumLength(50)
                            .Must((m, x) => HasParsableComparisonValue(filterPredicatesMaterialized, m, x))
                            .WithMessage(x =>
                                x.IsLiteral
                                    ? SimpleTradingStrings.NullNotAllowed
                                    : SimpleTradingStrings.ValueNotAllowed)
                            .WithName(SimpleTradingStrings.ComparisonValue);
                    });
            })
            .WithName(SimpleTradingStrings.Field);
    }

    private static bool HasParsableComparisonValue(IEnumerable<IFilterPredicate<Trade>> filterPredicates,
        FilterModel m, string comparisonValue)
    {
        return filterPredicates
            .First(p => p.Match(m.PropertyName, m.Operator))
            .CanParse(comparisonValue, m.IsLiteral);
    }
}