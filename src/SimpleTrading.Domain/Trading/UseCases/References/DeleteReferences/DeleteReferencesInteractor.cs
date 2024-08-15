using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.References.DeleteReferences;

public class DeleteReferencesInteractor(TradingDbContext dbContext) : BaseInteractor, IDeleteReferences
{
    public async Task<OneOf<Completed<ushort>, NotFound>> Execute(DeleteReferencesRequestModel model)
    {
        var trade = await dbContext.Trades.FindAsync(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        var referencesCount = (ushort)trade.References.Count;
        dbContext.References.RemoveRange(trade.References);
        await dbContext.SaveChangesAsync();

        return Completed(referencesCount);
    }
}