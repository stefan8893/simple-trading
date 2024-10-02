using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.References.DeleteReference;

public class DeleteReferenceInteractor(ITradeRepository tradeRepository, UowCommit uowCommit)
    : InteractorBase, IInteractor<DeleteReferenceRequestModel, OneOf<Completed, NotFound>>
{
    public async ValueTask<OneOf<Completed, NotFound>> Execute(DeleteReferenceRequestModel model)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        var reference = trade.References.SingleOrDefault(x => x.Id == model.ReferenceId);
        if (reference is null)
            return Completed();

        trade.References.Remove(reference);
        await uowCommit();

        return Completed();
    }
}