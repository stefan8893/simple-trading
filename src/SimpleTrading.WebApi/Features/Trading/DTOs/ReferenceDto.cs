using FluentValidation;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.WebApi.Features.Trading.DTOs;

public enum ReferenceTypeDto
{
    TradingView,
    Other
}

public record ReferenceDto
{
    public ReferenceTypeDto? Type { get; set; }
    public string? Link { get; set; }
    public string? Notes { get; set; }
}

public class ReferenceDtoValidator : AbstractValidator<ReferenceDto>
{
    public ReferenceDtoValidator()
    {
        RuleFor(x => x.Type)
            .NotNull()
            .WithName(SimpleTradingStrings.ReferenceType);

        RuleFor(x => x.Link)
            .NotNull()
            .WithName(SimpleTradingStrings.Link);
    }
}