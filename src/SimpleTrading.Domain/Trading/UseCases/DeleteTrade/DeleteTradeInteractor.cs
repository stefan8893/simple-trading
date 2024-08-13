using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.DeleteTrade;

public class DeleteTradeInteractor(TradingDbContext dbContext) : BaseInteractor, IDeleteTrade
{
    public async Task<OneOf<Completed, NotFound>> Execute(Guid tradeId)
    {
        var trade = await dbContext.Trades.FindAsync(tradeId);
        if (trade is null)
            return NotFound<Trade>(tradeId);

        dbContext.Remove(trade);
        await dbContext.SaveChangesAsync();

        return Completed();
    }
}