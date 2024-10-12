using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.References.DeleteReferences;

[UsedImplicitly]
public class DeleteReferencesInteractor(ITradeRepository tradeRepository, UowCommit uowCommit)
    : InteractorBase, IInteractor<DeleteReferencesRequestModel, OneOf<Completed<ushort>, NotFound>>
{
    public async Task<OneOf<Completed<ushort>, NotFound>> Execute(DeleteReferencesRequestModel model)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        var referencesCount = (ushort) trade.References.Count;
        tradeRepository.RemoveReferences(trade.References);
        await uowCommit();

        return Completed(referencesCount);
    }
}