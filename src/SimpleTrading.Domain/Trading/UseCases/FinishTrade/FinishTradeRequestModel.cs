using FluentValidation;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.FinishTrade;

public record FinishTradeRequestModel(Guid TradeId, Result Result, decimal Balance, decimal ExitPrice, DateTime FinishedAt);

public class FinishTradeRequestModelValidator : AbstractValidator<FinishTradeRequestModel>
{
    public FinishTradeRequestModelValidator()
    {
        RuleFor(x => x.Result)
            .IsInEnum()
            .WithName(SimpleTradingStrings.Result);
    }
}