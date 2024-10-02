using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.References.AddReference;

public record AddReferenceRequestModel(Guid TradeId, ReferenceType Type, string Link, string? Notes = null)
    : ReferenceRequestModel(Type, Link, Notes);

[UsedImplicitly]
public class AddReferenceRequestModelValidator : AbstractValidator<AddReferenceRequestModel>
{
    public AddReferenceRequestModelValidator(ReferenceRequestModelValidator referenceRequestModelValidator)
    {
        RuleFor(x => x)
            .SetValidator(referenceRequestModelValidator);
    }
}