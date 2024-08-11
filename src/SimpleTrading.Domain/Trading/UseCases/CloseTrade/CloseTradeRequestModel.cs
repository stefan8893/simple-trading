using FluentValidation;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.CloseTrade;

public record CloseTradeRequestModel(
    Guid TradeId,
    DateTime Closed,
    decimal Balance,
    ResultModel? Result = null,
    decimal? ExitPrice = null);

public class CloseTradeRequestModelValidator : AbstractValidator<CloseTradeRequestModel>
{
    public CloseTradeRequestModelValidator()
    {
        RuleFor(x => x.Result)
            .IsInEnum()
            .WithName(SimpleTradingStrings.Result)
            .When(x => x.Result.HasValue);

        RuleFor(x => x.ExitPrice)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.ExitPrice)
            .When(x => x.ExitPrice.HasValue);
    }
}