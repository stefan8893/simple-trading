using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

using AddTradeResponse =
    OneOf<Completed<AddTradeResponseModel>,
        BadInput,
        NotFound,
        BusinessError>;

[UsedImplicitly]
public class AddTradeInteractor(
    ITradeRepository tradeRepository,
    UowCommit uowCommit,
    UtcNow utcNow)
    : InteractorBase, IInteractor<AddTradeRequestModel, AddTradeResponse>
{
    public async Task<AddTradeResponse> Execute(AddTradeRequestModel model)
    {
        var asset = await tradeRepository.FindAsset(model.AssetId);
        if (asset is null)
            return NotFound<Asset>(model.AssetId);

        var profile = await tradeRepository.FindProfile(model.ProfileId);
        if (profile is null)
            return NotFound<Profile>(model.ProfileId);

        var currency = await tradeRepository.FindCurrency(model.CurrencyId);
        if (currency is null)
            return NotFound<Currency>(model.CurrencyId);

        return await AddTrade(model, asset, profile, currency);
    }

    private async Task<AddTradeResponse> AddTrade(AddTradeRequestModel model, Asset asset, Profile profile,
        Currency currency)
    {
        var trade = CreateTrade(model, asset, profile, currency);

        var potentiallyClosedTrade = TryCloseTrade(trade, model);

        if (potentiallyClosedTrade.Value is BusinessError businessError)
            return businessError;

        tradeRepository.Add(trade);
        await uowCommit();

        return potentiallyClosedTrade.Match<AddTradeResponse>(
            x => Completed(new AddTradeResponseModel(x.Data.TradeId,
                x.Data.Result?.ToResultModel(),
                x.Data.Result?.Performance,
                x.Data.Warnings)),
            x => Completed(AddTradeResponseModel.From(x.Trade, [])),
            x => x);
    }

    private Trade CreateTrade(AddTradeRequestModel model, Asset asset, Profile profile, Currency currency)
    {
        var newTrade = new Trade
        {
            Id = Guid.NewGuid(),
            AssetId = asset.Id,
            Asset = asset,
            ProfileId = profile.Id,
            Profile = profile,
            Size = model.Size,
            Opened = model.Opened.UtcDateTime,
            CurrencyId = currency.Id,
            Currency = currency,
            PositionPrices = new PositionPrices
            {
                Entry = model.EntryPrice,
                StopLoss = model.StopLoss,
                TakeProfit = model.TakeProfit,
                Exit = model.ExitPrice
            },
            Notes = model.Notes,
            Created = utcNow()
        };

        foreach (var m in model.References)
            newTrade.References.Add(new Reference
            {
                Id = Guid.NewGuid(),
                TradeId = newTrade.Id,
                Trade = newTrade,
                Type = m.Type,
                Link = new Uri(m.Link),
                Notes = m.Notes,
                Created = utcNow()
            });

        return newTrade;
    }

    private OneOf<Completed<CloseTradeResult>, NothingToClose, BusinessError> TryCloseTrade(
        Trade trade,
        AddTradeRequestModel model)
    {
        return model switch
        {
            {Balance: not null, Closed: not null} => Map(Close()),
            {Balance: null, Closed: null} => new NothingToClose(trade),
            _ => BusinessError(trade.Id, SimpleTradingStrings.ClosedTradeNeedsClosedAndBalance)
        };

        OneOf<Completed<CloseTradeResult>, BusinessError> Close()
        {
            return trade.Close(new CloseTradeConfiguration(
                model.Closed!.Value.UtcDateTime,
                model.Balance!.Value,
                utcNow)
            {
                ExitPrice = model.ExitPrice,
                ManuallyEnteredResult = model.ManuallyEnteredResult
            });
        }

        OneOf<Completed<CloseTradeResult>, NothingToClose, BusinessError> Map(
            OneOf<Completed<CloseTradeResult>, BusinessError> closeTradeResult)
        {
            return closeTradeResult
                .Match<OneOf<Completed<CloseTradeResult>, NothingToClose, BusinessError>>(
                    x => x,
                    x => x);
        }
    }

    private record NothingToClose(Trade Trade);
}