using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.WebApi.Features.Trading.Dto.Reference;

public record AddReferenceDto
{
    public string? Link { get; set; }
    public string? Notes { get; set; }
}

[UsedImplicitly]
public class AddReferenceDtoValidator : AbstractValidator<AddReferenceDto>
{
    public AddReferenceDtoValidator()
    {
        RuleFor(x => x.Link)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage(SimpleTradingStrings.InvalidLink)
            .WithName(SimpleTradingStrings.Link);
    }
}