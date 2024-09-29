using FluentValidation;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public class SearchQueryDto : IPagination
{
    public List<string>? Sort { get; set; }

    /// <example>balance -gt [100]</example>
    /// <example>size -eq [5000]</example>
    /// <example>opened -ge [2024-08-19T00:00:00+02:00]</example>
    public List<string>? Filter { get; set; }

    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

public class SearchQueryValidator : AbstractValidator<SearchQueryDto>
{
    public SearchQueryValidator(PropertyFilterValidator propertyFilterValidator)
    {
        RuleForEach(x => x.Filter)
            .SetValidator(propertyFilterValidator);
    }
}

public class PropertyFilterValidator : AbstractValidator<string>
{
    public PropertyFilterValidator()
    {
        RuleFor(x => x)
            .Matches(TradesController.PropertyFilterSyntaxRegex().ToString())
            .WithMessage(SimpleTradingStrings.InvalidFilterFormat)
            .When(x => !string.IsNullOrWhiteSpace(x));
    }
}