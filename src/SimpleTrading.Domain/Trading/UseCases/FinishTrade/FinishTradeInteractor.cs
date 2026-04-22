using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.FinishTrade;

using FinishTradeResponse =
    OneOf<Completed<FinishTradeResponseModel>, NotFound, Conflict>;

[UsedImplicitly]
public class FinishTradeInteractor(
    ITradeRepository tradeRepository,
    UowCommit uowCommit,
    UtcNow utcNow)
    : InteractorBase, IInteractor<FinishTradeRequestModel, FinishTradeResponse>
{
    public async Task<FinishTradeResponse> Execute(FinishTradeRequestModel model, CancellationToken cancellationToken)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        return await FinishTrade(trade, model);
    }

    private async Task<FinishTradeResponse> FinishTrade(Trade trade, FinishTradeRequestModel model)
    {
        var finishedTradeDto = new FinishTradeConfiguration(model.Finished.UtcDateTime,
            model.ProfitLoss,
            utcNow)
        {
            ExitPrice = model.ExitPrice,
            ManuallyEnteredResult = model.ManuallyEnteredResult
        };

        var result = trade.Finish(finishedTradeDto);

        if (result.Value is Completed<FinishTradeResult>)
            await uowCommit();

        return result.Match<FinishTradeResponse>(
            completed => Completed(Map(completed.Data)),
            conflict => conflict
        );

        FinishTradeResponseModel Map(FinishTradeResult finishTradeResult)
        {
            return new FinishTradeResponseModel(finishTradeResult.TradeId,
                finishTradeResult.Result?.ToResultModel(),
                finishTradeResult.Result?.Performance,
                finishTradeResult.Warnings);
        }
    }
}