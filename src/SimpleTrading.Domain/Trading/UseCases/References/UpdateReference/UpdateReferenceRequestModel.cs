using FluentValidation;
using JetBrains.Annotations;
using OneOf;
using OneOf.Types;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.References.UpdateReference;

public record UpdateReferenceRequestModel
{
    public required Guid TradeId { get; init; }
    public required Guid ReferenceId { get; init; }
    public ReferenceType? Type { get; init; }
    public string? Link { get; init; }
    public OneOf<string?, None> Notes { get; init; }
}

[UsedImplicitly]
public class UpdateReferenceRequestModelValidator : AbstractValidator<UpdateReferenceRequestModel>
{
    public UpdateReferenceRequestModelValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithName(SimpleTradingStrings.ReferenceType)
            .When(x => x.Type.HasValue);

        RuleFor(x => x.Link)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage(SimpleTradingStrings.InvalidLink)
            .When(x => x.Link is not null);

        RuleFor(x => x.Notes.AsT0)
            .MaximumLength(4000)
            .WithName(SimpleTradingStrings.Notes)
            .OverridePropertyName(x => x.Notes)
            .When(x => x.Notes is {IsT0: true, AsT0: not null});
    }
}