using FluentValidation;
using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.CloseTrade;

using CloseTradeResponse =
    OneOf<Completed<CloseTradeResponseModel>, BadInput, NotFound,
        BusinessError>;

public class CloseTradeInteractor(
    IValidator<CloseTradeRequestModel> validator,
    TradingDbContext dbContext,
    UtcNow utcNow)
    : BaseInteractor, ICloseTrade
{
    public async Task<CloseTradeResponse> Execute(CloseTradeRequestModel model)
    {
        var validation = await validator.ValidateAsync(model);
        if (!validation.IsValid)
            return BadInput(validation);

        var trade = await dbContext.FindAsync<Trade>(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        return await CloseTrade(trade, model);
    }

    private async Task<CloseTradeResponse> CloseTrade(Trade trade, CloseTradeRequestModel model)
    {
        var closeTradeDto = new Trade.CloseTradeDto(model.Closed.UtcDateTime,
            model.Balance,
            utcNow)
        {
            ExitPrice = model.ExitPrice,
            Result = model.Result
        };

        var result = trade.Close(closeTradeDto);

        if (result.Value is Completed)
            await dbContext.SaveChangesAsync();

        var closeTradeResponseModel = new CloseTradeResponseModel(trade.Result?.ToResultModel(),
            trade.Result?.Performance);

        return result.Match<CloseTradeResponse>(
            completed => Completed(closeTradeResponseModel, completed.Warnings),
            businessError => businessError
        );
    }
}