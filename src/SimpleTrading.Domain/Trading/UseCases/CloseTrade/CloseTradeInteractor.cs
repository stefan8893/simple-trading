﻿using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.CloseTrade;

using CloseTradeResponse =
    OneOf<Completed<CloseTradeResponseModel>, BadInput, NotFound,
        BusinessError>;

[UsedImplicitly]
public class CloseTradeInteractor(
    ITradeRepository tradeRepository,
    UowCommit uowCommit,
    UtcNow utcNow)
    : InteractorBase, IInteractor<CloseTradeRequestModel, CloseTradeResponse>
{
    public async Task<CloseTradeResponse> Execute(CloseTradeRequestModel model)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        return await CloseTrade(trade, model);
    }

    private async Task<CloseTradeResponse> CloseTrade(Trade trade, CloseTradeRequestModel model)
    {
        var closeTradeDto = new CloseTradeConfiguration(model.Closed.UtcDateTime,
            model.Balance,
            utcNow)
        {
            ExitPrice = model.ExitPrice,
            ManuallyEnteredResult = model.ManuallyEnteredResult
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