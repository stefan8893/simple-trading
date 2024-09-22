using FluentValidation;
using SimpleTrading.Domain.Resources;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading.Dto.Reference;

public record UpdateReferenceDto
{
    public ReferenceTypeDto? Type { get; set; }
    public string? Link { get; set; }
    public UpdateValue<string?>? Notes { get; set; }
}

public class UpdateReferenceDtoValidator : AbstractValidator<UpdateReferenceDto>
{
    public UpdateReferenceDtoValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithName(SimpleTradingStrings.ReferenceType)
            .When(x => x.Type.HasValue);
    }
}