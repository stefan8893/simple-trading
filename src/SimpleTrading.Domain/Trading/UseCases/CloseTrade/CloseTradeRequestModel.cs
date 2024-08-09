using FluentValidation;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.CloseTrade;

public record CloseTradeRequestModel(Guid TradeId, Result Result, decimal Balance, decimal ExitPrice, DateTime Closed);

public class CloseTradeRequestModelValidator : AbstractValidator<CloseTradeRequestModel>
{
    public CloseTradeRequestModelValidator()
    {
        RuleFor(x => x.Result)
            .IsInEnum()
            .WithName(SimpleTradingStrings.Result);

        RuleFor(x => x.ExitPrice)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.ExitPrice);
    }
}