using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.DeleteTrade;

[UsedImplicitly]
public class DeleteTradeInteractor(ITradeRepository tradeRepository, UowCommit uowCommit)
    : InteractorBase, IInteractor<Guid, OneOf<Completed, NotFound>>
{
    public async Task<OneOf<Completed, NotFound>> Execute(Guid tradeId, CancellationToken cancellationToken)
    {
        var trade = await tradeRepository.Find(tradeId);
        if (trade is null)
            return NotFound<Trade>(tradeId);

        tradeRepository.Remove(trade);
        await uowCommit();

        return Completed();
    }
}