using FluentValidation;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.WebApi.Features.Trading.Dto.Reference;

public record AddReferenceDto
{
    public ReferenceTypeDto? Type { get; set; }
    public string? Link { get; set; }
    public string? Notes { get; set; }
}

public class AddReferenceDtoValidator : AbstractValidator<AddReferenceDto>
{
    public AddReferenceDtoValidator()
    {
        RuleFor(x => x.Type)
            .NotNull()
            .WithName(SimpleTradingStrings.ReferenceType);

        RuleFor(x => x.Link)
            .NotNull()
            .WithName(SimpleTradingStrings.Link);
    }
}