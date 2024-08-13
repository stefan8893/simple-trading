using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.GetTrade;

public class GetTradeInteractor(TradingDbContext dbContext) : BaseInteractor, IGetTrade
{
    public async Task<OneOf<TradeResponseModel, NotFound>> Execute(Guid tradeId)
    {
        var trade = await dbContext.Trades.FindAsync(tradeId);
        if (trade is null)
            return NotFound<Trade>(tradeId);

        var userSettings = await dbContext.GetUserSettings();

        return TradeResponseModel.From(trade, userSettings.TimeZone);
    }
}