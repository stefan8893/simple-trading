using FluentValidation;
using OneOf;
using OneOf.Types;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.Domain.Trading.UseCases.Shared.Validators;

namespace SimpleTrading.Domain.Trading.UseCases.UpdateTrade;

public record UpdateTradeRequestModel
{
    public required Guid TradeId { get; init; }
    public Guid? AssetId { get; init; }
    public Guid? ProfileId { get; init; }
    public DateTimeOffset? Opened { get; init; }
    public DateTimeOffset? Closed { get; set; }
    public decimal? Size { get; init; }
    public OneOf<ResultModel?, None> Result { get; set; }
    public decimal? Balance { get; set; }
    public Guid? CurrencyId { get; init; }
    public decimal? EntryPrice { get; init; }
    public OneOf<decimal?, None> StopLoss { get; set; }
    public OneOf<decimal?, None> TakeProfit { get; set; }
    public OneOf<decimal?, None> ExitPrice { get; set; }
    public OneOf<string?, None> Notes { get; set; }
}

public class UpdateTradeRequestModelValidator : AbstractValidator<UpdateTradeRequestModel>
{
    public UpdateTradeRequestModelValidator(OpenedLessThanOneDayInTheFutureValidator openedLessThanOneDayInTheFutureValidator)
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Asset)
            .When(x => x.AssetId.HasValue);

        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Profile)
            .When(x => x.ProfileId.HasValue);

        RuleFor(x => x.Opened.HasValue ? x.Opened.Value.DateTime : DateTime.MinValue)
            .GreaterThanOrEqualTo(Constants.MinDate)
            .OverridePropertyName(x => x.Opened)
            .WithName(SimpleTradingStrings.Opened)
            .When(x => x.Opened.HasValue);

        RuleFor(x => x.Opened)
            .SetValidator(openedLessThanOneDayInTheFutureValidator);

        RuleFor(x => x.Size)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.TradeSize)
            .When(x => x.Size.HasValue);

        RuleFor(x => x.Result.AsT0)
            .IsInEnum()
            .WithName(SimpleTradingStrings.Result)
            .OverridePropertyName(x => x.Result)
            .When(x => x.Result is {IsT0: true, AsT0: not null});

        RuleFor(x => x.EntryPrice)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.EntryPrice)
            .When(x => x.EntryPrice.HasValue);

        RuleFor(x => x.StopLoss.AsT0)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.StopLoss)
            .OverridePropertyName(x => x.StopLoss)
            .When(x => x.StopLoss is {IsT0: true, AsT0: not null});

        RuleFor(x => x.TakeProfit.AsT0)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.TakeProfit)
            .OverridePropertyName(x => x.TakeProfit)
            .When(x => x.TakeProfit is {IsT0: true, AsT0: not null});

        RuleFor(x => x.ExitPrice.AsT0)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.ExitPrice)
            .OverridePropertyName(x => x.ExitPrice)
            .When(x => x.ExitPrice is {IsT0: true, AsT0: not null});

        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Currency)
            .When(x => x.CurrencyId.HasValue);

        RuleFor(x => x.Notes.AsT0)
            .MaximumLength(4000)
            .WithName(SimpleTradingStrings.Notes)
            .OverridePropertyName(x => x.Notes)
            .When(x => x.Notes is {IsT0: true, AsT0: not null});
    }
}