using FluentValidation;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.CloseTrade;

public record CloseTradeRequestModel(
    Guid TradeId,
    DateTimeOffset Closed,
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