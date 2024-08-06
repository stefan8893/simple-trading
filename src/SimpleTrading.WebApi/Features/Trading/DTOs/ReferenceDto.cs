using FluentValidation;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.WebApi.Features.Trading.DTOs;

public record ReferenceDto
{
    public ReferenceType? Type { get; set; }
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