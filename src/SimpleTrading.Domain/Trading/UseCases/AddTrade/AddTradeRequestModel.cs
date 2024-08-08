using FluentValidation;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

public record AddTradeRequestModel
{
    public Guid AssetId { get; init; }
    public Guid ProfileId { get; init; }
    public DateTime OpenedAt { get; init; }
    public DateTime? FinishedAt { get; set; }
    public decimal Size { get; init; }
    public Result? Result { get; set; }
    public decimal? Balance { get; set; }
    public Guid CurrencyId { get; init; }
    public decimal EntryPrice { get; init; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public string? Notes { get; set; }
    public IReadOnlyList<ReferenceModel> References { get; set; } = [];

    public record ReferenceModel(ReferenceType Type, string Link, string? Notes = null);
}

public class AddTradeRequestModelValidator : AbstractValidator<AddTradeRequestModel>
{
    public AddTradeRequestModelValidator(ReferenceModelValidator referenceModelValidator)
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Asset);

        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Profile);

        RuleFor(x => x.OpenedAt)
            .GreaterThanOrEqualTo(Constants.MinDate)
            .WithName(SimpleTradingStrings.OpenedAt);

        RuleFor(x => x.Size)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.TradeSize);

        RuleFor(x => x.Result)
            .IsInEnum()
            .WithName(SimpleTradingStrings.Result)
            .When(x => x.Result is not null);

        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Currency);

        RuleForEach(x => x.References)
            .SetValidator(referenceModelValidator);
    }
}

public class ReferenceModelValidator : AbstractValidator<AddTradeRequestModel.ReferenceModel>
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