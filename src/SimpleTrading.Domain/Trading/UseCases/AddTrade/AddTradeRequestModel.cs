using FluentValidation;
using OneOf;
using OneOf.Types;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.Domain.Trading.UseCases.Shared.Validators;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

public record AddTradeReferenceRequestModel(Guid Id, ReferenceType Type, string Link, string? Notes = null)
    : ReferenceModel(Id, Type, Link, Notes);

public record AddTradeRequestModel
{
    public required Guid AssetId { get; init; }
    public required Guid ProfileId { get; init; }
    public required DateTimeOffset Opened { get; init; }
    public DateTimeOffset? Closed { get; set; }
    public required decimal Size { get; init; }
    public OneOf<ResultModel?, None> ManuallyEnteredResult { get; set; }
    public decimal? Balance { get; set; }
    public required Guid CurrencyId { get; init; }
    public required decimal EntryPrice { get; init; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public decimal? ExitPrice { get; set; }
    public string? Notes { get; set; }
    public IReadOnlyList<ReferenceRequestModel> References { get; set; } = [];
}

public class AddTradeRequestModelValidator : AbstractValidator<AddTradeRequestModel>
{
    public AddTradeRequestModelValidator(
        OpenedLessThanOneDayInTheFutureValidator openedLessThanOneDayInTheFutureValidator,
        ReferenceRequestModelValidator referenceRequestModelValidator)
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Asset);

        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Profile);

        RuleFor(x => x.Opened.DateTime)
            .GreaterThanOrEqualTo(Constants.MinDate)
            .OverridePropertyName(x => x.Opened)
            .WithName(SimpleTradingStrings.Opened);

        RuleFor(x => (DateTimeOffset?) x.Opened)
            .SetValidator(openedLessThanOneDayInTheFutureValidator);

        RuleFor(x => x.Size)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.TradeSize);

        RuleFor(x => x.ManuallyEnteredResult.AsT0)
            .IsInEnum()
            .WithName(SimpleTradingStrings.Result)
            .OverridePropertyName(x => x.ManuallyEnteredResult)
            .When(x => x.ManuallyEnteredResult is {IsT0: true, AsT0: not null});

        RuleFor(x => x.ManuallyEnteredResult.AsT0)
            .Empty()
            .WithName(SimpleTradingStrings.Result)
            .OverridePropertyName(x => x.ManuallyEnteredResult)
            .WithMessage(SimpleTradingStrings.BalanceAndClosedMustBePresentWhenOverridingResult)
            .When(x => !(x.Balance.HasValue && x.Closed.HasValue) && x.ManuallyEnteredResult.IsT0);

        RuleFor(x => x.Balance)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Balance)
            .WithMessage(string.Format(SimpleTradingStrings.XMustNotBeEmptyIfYIsPresent, "{PropertyName}",
                SimpleTradingStrings.Closed))
            .When(x => x.Closed.HasValue);

        RuleFor(x => x.Closed)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Closed)
            .WithMessage(string.Format(SimpleTradingStrings.XMustNotBeEmptyIfYIsPresent, "{PropertyName}",
                SimpleTradingStrings.Balance))
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
            .SetValidator(referenceRequestModelValidator);
    }
}