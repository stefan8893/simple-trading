using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.References.DeleteReferences;

public class DeleteReferencesInteractor(ITradeRepository tradeRepository, UowCommit uowCommit)
    : BaseInteractor, IDeleteReferences
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