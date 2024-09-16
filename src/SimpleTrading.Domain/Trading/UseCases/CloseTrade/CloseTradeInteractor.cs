using FluentValidation;
using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.CloseTrade;

using CloseTradeResponse =
    OneOf<Completed<CloseTradeResponseModel>, BadInput, NotFound,
        BusinessError>;

public class CloseTradeInteractor(
    IValidator<CloseTradeRequestModel> validator,
    ITradeRepository tradeRepository,
    UowCommit uowCommit,
    UtcNow utcNow)
    : BaseInteractor, ICloseTrade
{
    public async Task<CloseTradeResponse> Execute(CloseTradeRequestModel model)
    {
        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
            return BadInput(validationResult);

        var trade = await tradeRepository.Find(model.TradeId);
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
            await uowCommit();

        var closeTradeResponseModel = new CloseTradeResponseModel(trade.Id,
            trade.Result?.ToResultModel(),
            trade.Result?.Performance);

        return result.Match<CloseTradeResponse>(
            completed => Completed(closeTradeResponseModel, completed.Warnings),
            businessError => businessError
        );
    }
}