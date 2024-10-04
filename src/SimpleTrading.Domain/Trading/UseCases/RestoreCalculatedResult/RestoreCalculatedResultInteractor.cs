using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;

using RestoreCalculatedResultResponse =
    OneOf<Completed<RestoreCalculatedResultResponseModel>, NotFound, BusinessError>;

[UsedImplicitly]
public class RestoreCalculatedResultInteractor(ITradeRepository tradeRepository, UtcNow utcNow, UowCommit uowCommit)
    : InteractorBase, IInteractor<RestoreCalculatedResultRequestModel, RestoreCalculatedResultResponse>
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
            .Match<RestoreCalculatedResultResponse>(
                completed => Completed(new RestoreCalculatedResultResponseModel(
                    completed.Data.TradeId,
                    completed.Data.Result?.ToResultModel(),
                    completed.Data.Result?.Performance,
                    completed.Data.Warnings)),
                businessError => businessError);
    }
}