using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.Shared;

public record ReferenceRequestModel(string Link, string? Notes = null);

[UsedImplicitly]
public class ReferenceRequestModelValidator : AbstractValidator<ReferenceRequestModel>
{
    public ReferenceRequestModelValidator()
    {
        RuleFor(x => x.Link)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage(SimpleTradingStrings.InvalidLink);

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .WithName(SimpleTradingStrings.Notes);
    }
}