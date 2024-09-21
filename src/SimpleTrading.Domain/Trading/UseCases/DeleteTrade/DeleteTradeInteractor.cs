using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.DeleteTrade;

public class DeleteTradeInteractor(ITradeRepository tradeRepository, UowCommit uowCommit) : InteractorBase, IDeleteTrade
{
    public async Task<OneOf<Completed, NotFound>> Execute(DeleteTradeRequestModel model)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        tradeRepository.Remove(trade);
        await uowCommit();

        return Completed();
    }
}