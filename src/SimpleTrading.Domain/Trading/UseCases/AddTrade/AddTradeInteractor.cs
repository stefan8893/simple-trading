using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

using AddTradeResponse =
    OneOf<Completed<AddTradeResponseModel>,
        NotFound,
        Conflict>;

[UsedImplicitly]
public class AddTradeInteractor(
    ITradeRepository tradeRepository,
    IAssetRepository assetRepository,
    IProfileRepository profileRepository,
    ICurrencyRepository currencyRepository,
    UowCommit uowCommit,
    UtcNow utcNow)
    : InteractorBase, IInteractor<AddTradeRequestModel, AddTradeResponse>
{
    public async Task<AddTradeResponse> Execute(AddTradeRequestModel model, CancellationToken cancellationToken)
    {
        var asset = await assetRepository.Get(model.AssetId);
        var profile = await profileRepository.Get(model.ProfileId);
        var currency = await currencyRepository.Get(model.CurrencyId);

        return await AddTrade(model, asset, profile, currency);
    }

    private async Task<AddTradeResponse> AddTrade(AddTradeRequestModel model, Asset asset, Profile profile,
        Currency currency)
    {
        var trade = CreateTrade(model, asset, profile, currency);

        var potentiallyFinishedTrade = TryFinishTrade(trade, model);

        if (potentiallyFinishedTrade.Value is Conflict conflict)
            return conflict;

        tradeRepository.Add(trade);
        if (!model.DryRun)
            await uowCommit();

        return potentiallyFinishedTrade.Match<AddTradeResponse>(
            x => Completed(new AddTradeResponseModel(x.Data.TradeId,
                model.DryRun,
                x.Data.Result?.ToResultModel(),
                x.Data.Result?.Performance,
                x.Data.Warnings)),
            x => Completed(AddTradeResponseModel.From(x.Trade, [], model.DryRun)),
            x => x);
    }

    private Trade CreateTrade(AddTradeRequestModel model, Asset asset, Profile profile, Currency currency)
    {
        var newTrade = new Trade
        {
            Id = Guid.CreateVersion7(),
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
                Id = Guid.CreateVersion7(),
                TradeId = newTrade.Id,
                Trade = newTrade,
                Link = new Uri(m.Link),
                Notes = m.Notes,
                Created = utcNow()
            });

        return newTrade;
    }

    private OneOf<Completed<FinishTradeResult>, NothingToFinish, Conflict> TryFinishTrade(
        Trade trade,
        AddTradeRequestModel model)
    {
        return model switch
        {
            {ProfitLoss: not null, Finished: not null} => Map(Finish()),
            {ProfitLoss: null, Finished: null} => new NothingToFinish(trade),
            _ => Conflict(trade.Id, SimpleTradingStrings.FinishedTradeNeedsFinishedAndProfitLoss)
        };

        OneOf<Completed<FinishTradeResult>, Conflict> Finish()
        {
            return trade.Finish(new FinishTradeConfiguration(
                model.Finished!.Value.UtcDateTime,
                model.ProfitLoss!.Value,
                utcNow)
            {
                ExitPrice = model.ExitPrice,
                ManuallyEnteredResult = model.ManuallyEnteredResult
            });
        }

        OneOf<Completed<FinishTradeResult>, NothingToFinish, Conflict> Map(
            OneOf<Completed<FinishTradeResult>, Conflict> finishTradeResult)
        {
            return finishTradeResult
                .Match<OneOf<Completed<FinishTradeResult>, NothingToFinish, Conflict>>(
                    x => x,
                    x => x);
        }
    }

    private record NothingToFinish(Trade Trade);
}