using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.Domain.User.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.GetTrade;

[UsedImplicitly]
public class GetTradeInteractor(ITradeRepository tradeRepository, IUserSettingsRepository userSettingsRepository)
    : InteractorBase, IInteractor<Guid, OneOf<TradeResponseModel, NotFound>>
{
    public async Task<OneOf<TradeResponseModel, NotFound>> Execute(Guid tradeId, CancellationToken cancellationToken)
    {
        var trade = await tradeRepository.Find(tradeId);
        if (trade is null)
            return NotFound<Trade>(tradeId);

        var userSettings = await userSettingsRepository.GetUserSettings();

        return TradeResponseModel.From(trade, userSettings.TimeZone);
    }
}