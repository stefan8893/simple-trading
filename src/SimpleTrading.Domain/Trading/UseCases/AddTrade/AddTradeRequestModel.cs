using FluentValidation;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

public record AddTradeRequestModel
{
    public required Guid AssetId { get; init; }
    public required Guid ProfileId { get; init; }
    public required DateTimeOffset Opened { get; init; }
    public DateTimeOffset? Closed { get; set; }
    public required decimal Size { get; init; }
    public ResultModel? Result { get; set; }
    public decimal? Balance { get; set; }
    public required Guid CurrencyId { get; init; }
    public required decimal EntryPrice { get; init; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public decimal? ExitPrice { get; set; }
    public string? Notes { get; set; }
    public IReadOnlyList<ReferenceModel> References { get; set; } = [];
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

        RuleFor(x => x.Opened)
            .GreaterThanOrEqualTo(Constants.MinDate)
            .WithName(SimpleTradingStrings.Opened);

        RuleFor(x => x.Size)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.TradeSize);

        RuleFor(x => x.Result)
            .IsInEnum()
            .WithName(SimpleTradingStrings.Result)
            .When(x => x.Result.HasValue);

        RuleFor(x => x.Balance)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Balance)
            .WithMessage(string.Format(SimpleTradingStrings.XMustNotBeEmptyIfYIsThere, "{PropertyName}", SimpleTradingStrings.Closed))
            .When(x => x.Closed.HasValue);

        RuleFor(x => x.Closed)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Closed)
            .WithMessage(string.Format(SimpleTradingStrings.XMustNotBeEmptyIfYIsThere, "{PropertyName}", SimpleTradingStrings.Balance))
            .When(x => x.Balance.HasValue);

        RuleFor(x => x.EntryPrice)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.EntryPrice);

        RuleFor(x => x.StopLoss)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.StopLoss)
            .When(x => x.StopLoss.HasValue);

        RuleFor(x => x.TakeProfit)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.TakeProfit)
            .When(x => x.TakeProfit.HasValue);

        RuleFor(x => x.ExitPrice)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.ExitPrice)
            .When(x => x.ExitPrice.HasValue);

        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Currency);

        RuleFor(x => x.Notes)
            .MaximumLength(4000)
            .WithName(SimpleTradingStrings.Notes);

        RuleForEach(x => x.References)
            .SetValidator(referenceModelValidator);
    }
}

