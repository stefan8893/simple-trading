using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.GetTrade;

public class GetTradeInteractor(ITradeRepository tradeRepository, IUserSettingsRepository userSettingsRepository)
    : BaseInteractor, IGetTrade
{
    public async Task<OneOf<TradeResponseModel, NotFound>> Execute(GetTradeRequestModel model)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        var userSettings = await userSettingsRepository.Get();

        return TradeResponseModel.From(trade, userSettings.TimeZone);
    }
}