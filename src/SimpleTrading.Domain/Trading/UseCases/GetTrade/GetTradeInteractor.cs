using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.Domain.User.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.GetTrade;

public class GetTradeInteractor(ITradeRepository tradeRepository, IUserSettingsRepository userSettingsRepository)
    : InteractorBase, IInteractor<GetTradeRequestModel, OneOf<TradeResponseModel, NotFound>>
{
    public async ValueTask<OneOf<TradeResponseModel, NotFound>> Execute(GetTradeRequestModel model)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        var userSettings = await userSettingsRepository.GetUserSettings();

        return TradeResponseModel.From(trade, userSettings.TimeZone);
    }
}