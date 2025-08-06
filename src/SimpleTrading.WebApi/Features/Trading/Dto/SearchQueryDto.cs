using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public class SearchQueryDto : IPagination
{
    public Guid? ProfileId { get; set; }
    public List<string>? Sort { get; set; }
    public List<string>? Filter { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

[UsedImplicitly]
public class SearchQueryValidator : AbstractValidator<SearchQueryDto>
{
    public SearchQueryValidator(PropertyFilterValidator propertyFilterValidator)
    {
        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Profile);
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
                    .Matches(TradesController.PropertyFilterSyntaxRegex().ToString())
                    .WithMessage(SimpleTradingStrings.InvalidFilterFormat)
                    .When(x => !string.IsNullOrWhiteSpace(x));
            })
            .When(x => x.Filter is not null);
    }
}