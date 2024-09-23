using FluentValidation;
using OneOf;
using OneOf.Types;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.CloseTrade;

public class CloseTradeRequestModel(
    Guid tradeId,
    DateTimeOffset closed,
    decimal balance)
{
    public Guid TradeId { get; init; } = tradeId;
    public DateTimeOffset Closed { get; init; } = closed;
    public decimal Balance { get; init; } = balance;
    public OneOf<ResultModel?, None> ManuallyEnteredResult { get; init; } = new None();
    public decimal? ExitPrice { get; init; }
}

public class CloseTradeRequestModelValidator : AbstractValidator<CloseTradeRequestModel>
{
    public CloseTradeRequestModelValidator()
    {
        RuleFor(x => x.ManuallyEnteredResult.AsT0)
            .IsInEnum()
            .WithName(SimpleTradingStrings.Result)
            .OverridePropertyName(x => x.ManuallyEnteredResult)
            .When(x => x.ManuallyEnteredResult is {IsT0: true, AsT0: not null});

        RuleFor(x => x.ExitPrice)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.ExitPrice)
            .When(x => x.ExitPrice.HasValue);
    }
}