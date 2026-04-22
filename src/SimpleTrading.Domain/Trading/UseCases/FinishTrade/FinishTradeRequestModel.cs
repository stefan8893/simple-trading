using FluentValidation;
using JetBrains.Annotations;
using OneOf;
using OneOf.Types;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.FinishTrade;

public class FinishTradeRequestModel(
    Guid tradeId,
    DateTimeOffset finished,
    decimal profitLoss)
{
    public Guid TradeId { get; init; } = tradeId;
    public DateTimeOffset Finished { get; init; } = finished;
    public decimal ProfitLoss { get; init; } = profitLoss;
    public OneOf<ResultModel?, None> ManuallyEnteredResult { get; init; } = new None();
    public decimal? ExitPrice { get; init; }
}

[UsedImplicitly]
public class FinishTradeRequestModelValidator : AbstractValidator<FinishTradeRequestModel>
{
    public FinishTradeRequestModelValidator()
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