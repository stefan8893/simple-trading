using FluentValidation;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

public record ReferenceModel(ReferenceType Type, string Link, string? Notes = null);

public class ReferenceModelValidator : AbstractValidator<ReferenceModel>
{
    public ReferenceModelValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithName(SimpleTradingStrings.ReferenceType);

        RuleFor(x => x.Link)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage(SimpleTradingStrings.InvalidLink);

        RuleFor(x => x.Notes)
            .MaximumLength(4000)
            .WithName(SimpleTradingStrings.Notes);
    }
}