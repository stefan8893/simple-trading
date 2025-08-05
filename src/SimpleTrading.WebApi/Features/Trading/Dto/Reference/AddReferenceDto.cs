using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.WebApi.Features.Trading.Dto.Reference;

public record AddReferenceDto
{
    public ReferenceTypeDto? Type { get; set; }
    public string? Link { get; set; }
    public string? Notes { get; set; }
}

[UsedImplicitly]
public class AddReferenceDtoValidator : AbstractValidator<AddReferenceDto>
{
    public AddReferenceDtoValidator()
    {
        RuleFor(x => x.Type)
            .NotNull()
            .WithName(SimpleTradingStrings.ReferenceType);

        RuleFor(x => x.Link)
            .NotNull()
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage(SimpleTradingStrings.InvalidLink)
            .WithName(SimpleTradingStrings.Link);
    }
}