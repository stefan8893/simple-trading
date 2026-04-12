using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.Shared;

public record ReferenceRequestModel(ReferenceType Type, string Link, string? Notes = null);

[UsedImplicitly]
public class ReferenceRequestModelValidator : AbstractValidator<ReferenceRequestModel>
{
    public ReferenceRequestModelValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithName(SimpleTradingStrings.ReferenceType);

        RuleFor(x => x.Link)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage(SimpleTradingStrings.InvalidLink);

        RuleFor(x => x.Link)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out var parsed) &&
                         parsed.Host.EndsWith("tradingview.com", StringComparison.OrdinalIgnoreCase))
            .WithMessage(SimpleTradingStrings.NotTradingViewLink)
            .When(x => x.Type == ReferenceType.TradingView);

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .WithName(SimpleTradingStrings.Notes);
    }
}