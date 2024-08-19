using FluentValidation;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertySorting;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades;

public record SortModel(string Property, bool Ascending = true);

public record FilterModel
{
    public required string PropertyName { get; init; }
    public required string Operator { get; init; }
    public required string ComparisonValue { get; init; }
}

public class SearchTradesRequestModel : Pagination
{
    public IReadOnlyList<SortModel> Sort { get; init; } = [];
    public IReadOnlyList<FilterModel> Filter { get; init; } = [];
}

public class SearchTradesRequestModelValidator : AbstractValidator<SearchTradesRequestModel>
{
    public SearchTradesRequestModelValidator(FilterModelValidator filterModelValidator,
        SortModelValidator sortModelValidator, PaginationValidator paginationValidator)
    {
        RuleForEach(x => x.Sort)
            .SetValidator(sortModelValidator);

        RuleForEach(x => x.Filter)
            .SetValidator(filterModelValidator);

        RuleFor(x => x)
            .SetValidator(paginationValidator);
    }
}

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

public class FilterModelValidator : AbstractValidator<FilterModel>
{
    public FilterModelValidator(
        IReadOnlyDictionary<string, IPropertyFilterComparisonVisitor<Trade>> comparisonByOperator)
    {
        RuleFor(x => x.PropertyName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(x => SupportedPropertyFilters.All.Contains(x))
            .WithMessage(SimpleTradingStrings.FilterNotSupported)
            .DependentRules(() =>
            {
                RuleFor(x => x.ComparisonValue)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .MaximumLength(50)
                    .Must((m, x) => PropertyFilterFactory.CanCreateFilter(m.PropertyName, x))
                    .WithMessage(SimpleTradingStrings.ValueNotAllowed)
                    .WithName(SimpleTradingStrings.ComparisonValue);
            })
            .WithName(SimpleTradingStrings.Field);

        RuleFor(x => x.Operator)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(comparisonByOperator.ContainsKey)
            .WithMessage(SimpleTradingStrings.OperatorNotSupported)
            .WithName(SimpleTradingStrings.Operator);
    }
}