using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;

using RestoreCalculatedResultResponse =
    OneOf<Completed<RestoreCalculatedResultResponseModel>, NotFound, BusinessError>;

public class RestoreCalculatedResultInteractor(ITradeRepository tradeRepository, UtcNow utcNow, UowCommit uowCommit)
    : InteractorBase, IRestoreCalculatedResult
{
    public async Task<RestoreCalculatedResultResponse> Execute(RestoreCalculatedResultRequestModel model)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        var result = trade.RestoreCalculatedResult(utcNow);

        if (result.Value is Completed)
            await uowCommit();

        return result
            .Match<RestoreCalculatedResultResponse>(completed => Completed(
                new RestoreCalculatedResultResponseModel(trade.Id, trade.Result?.ToResultModel(),
                    trade.Result?.Performance), completed.Warnings), businessError => businessError);
    }
}