using System.Text.RegularExpressions;
using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public partial class SearchQueryDto : IPagination
{
    public const string UnprocessableEntityRuleSet = "UnprocessableEntity";

    public Guid? ProfileId { get; set; }
    public List<string>? Sort { get; set; }
    public List<string>? Filter { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public SearchTradesRequestModel ToRequestModel()
    {
        var requestModel = new SearchTradesRequestModel
        {
            ProfileId = ProfileId!.Value,
            Sort = Sort?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(ParseSorting)
                .ToList() ?? [],
            Filter = Filter?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(ParseFilter)
                .ToList() ?? []
        };

        if (Page.HasValue)
            requestModel.Page = Page.Value;

        if (PageSize.HasValue)
            requestModel.PageSize = PageSize.Value;

        return requestModel;
    }

    internal static FilterModel ParseFilter(string filter)
    {
        var match = PropertyFilterSyntaxRegex().Match(filter);
        var comparisonValue = match.Groups["comparisonValue"];
        var literal = match.Groups["literal"];

        return new FilterModel
        {
            PropertyName = match.Groups["property"].Value,
            Operator = match.Groups["operator"].Value,
            ComparisonValue = comparisonValue.Success
                ? comparisonValue.Value
                : literal.Value,
            IsLiteral = !comparisonValue.Success && literal.Success
        };
    }

    internal static SortModel ParseSorting(string sortBy)
    {
        var sortByTrimmed = sortBy.Trim();

        return sortByTrimmed.StartsWith('-')
            ? new SortModel(sortByTrimmed[1..], false)
            : new SortModel(sortByTrimmed);
    }

    [GeneratedRegex(
        @"\s*(?<property>.*?)\s+\-(?<operator>.*?)\s+(?<literal>(?i)null|true|false(?-i)|\[(?<comparisonValue>.*?)\])\s*$")]
    public static partial Regex PropertyFilterSyntaxRegex();
}

[UsedImplicitly]
public class SearchQueryValidator : AbstractValidator<SearchQueryDto>
{
    public SearchQueryValidator(FilterModelValidator filterModelValidator, SortModelValidator sortModelValidator)
    {
        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Profile);

        RuleSet(SearchQueryDto.UnprocessableEntityRuleSet, () =>
        {
            RuleForEach(x => x.Sort)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Custom((sort, context) =>
                {
                    var validationResult = sortModelValidator.Validate(SearchQueryDto.ParseSorting(sort));
                    foreach (var error in validationResult.Errors)
                        context.AddFailure($"{context.PropertyPath}.{error.PropertyName}", error.ErrorMessage);
                });

            RuleForEach(x => x.Filter)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Custom((filter, context) =>
                {
                    var validationResult = filterModelValidator.Validate(SearchQueryDto.ParseFilter(filter));
                    foreach (var error in validationResult.Errors)
                        context.AddFailure($"{context.PropertyPath}.{error.PropertyName}", error.ErrorMessage);
                });

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(1000)
                .When(x => x.Page.HasValue)
                .WithName(SimpleTradingStrings.Page);

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(500)
                .When(x => x.PageSize.HasValue)
                .WithName(SimpleTradingStrings.PageSize);
        });
    }
}

[UsedImplicitly]
public class PropertyFilterValidator : AbstractValidator<SearchQueryDto>
{
    public PropertyFilterValidator()
    {
        RuleForEach(x => x.Filter)
            .ChildRules(filter =>
            {
                filter.RuleFor(x => x)
                    .Matches(SearchQueryDto.PropertyFilterSyntaxRegex().ToString())
                    .WithMessage(SimpleTradingStrings.InvalidFilterFormat)
                    .When(x => !string.IsNullOrWhiteSpace(x));
            })
            .When(x => x.Filter is not null);
    }
}