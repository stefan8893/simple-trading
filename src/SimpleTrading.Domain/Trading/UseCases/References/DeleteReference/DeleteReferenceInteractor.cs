using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.References.DeleteReference;

public class DeleteReferenceInteractor(TradingDbContext dbContext) : BaseInteractor, IDeleteReference
{
    public async Task<OneOf<Completed, NotFound>> Execute(DeleteReferenceRequestModel model)
    {
        var trade = await dbContext.Trades.FindAsync(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        var reference = trade.References.SingleOrDefault(x => x.Id == model.ReferenceId);
        if (reference is null)
            return Completed();

        trade.References.Remove(reference);
        await dbContext.SaveChangesAsync();

        return Completed();
    }
}