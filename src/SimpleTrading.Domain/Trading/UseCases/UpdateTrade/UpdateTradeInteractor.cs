using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.UpdateTrade;

using UpdateTradeResponse =
    OneOf<Completed<UpdateTradeResponseModel>,
        NotFound,
        Conflict>;

[UsedImplicitly]
public class UpdateTradeInteractor(
    ITradeRepository tradeRepository,
    IAssetRepository assetRepository,
    IProfileRepository profileRepository,
    ICurrencyRepository currencyRepository,
    UowCommit uowCommit,
    UtcNow utcNow)
    : InteractorBase, IInteractor<UpdateTradeRequestModel, UpdateTradeResponse>
{
    public async Task<UpdateTradeResponse> Execute(UpdateTradeRequestModel model, CancellationToken cancellationToken)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        return await UpdateTrade(model, trade);
    }

    private async Task<UpdateTradeResponse> UpdateTrade(UpdateTradeRequestModel model, Trade trade)
    {
        var updateEntitiesResult = await UpdateEntities(trade, model);
        var isCompleted = updateEntitiesResult.IsT0;
        if (!isCompleted)
            return updateEntitiesResult.AsT1;

        var updatePropertiesResult = UpdateTradeProperties(trade, model);
        if (updatePropertiesResult.Value is Conflict updatePropertiesConflict)
            return updatePropertiesConflict;

        var hasChanges = UpdatePositionPrices(trade, model);
        var finishTradeResult = FinishTrade(trade, model, hasChanges);

        if (finishTradeResult.Value is Conflict finishTradeConflict)
            return finishTradeConflict;

        await uowCommit();
        return finishTradeResult
            .Match<UpdateTradeResponse>(
                completed => Completed(new UpdateTradeResponseModel(completed.Data.TradeId,
                    completed.Data.Result?.ToResultModel(),
                    completed.Data.Result?.Performance,
                    completed.Data.Warnings)),
                nothingToClose => Completed(UpdateTradeResponseModel.From(nothingToClose.Trade, [])),
                conflict => conflict);
    }

    private async Task<OneOf<Completed, NotFound>> UpdateEntities(Trade trade,
        UpdateTradeRequestModel model)
    {
        if (model.AssetId.HasValue && model.AssetId.Value != trade.AssetId)
        {
            var newAsset = await assetRepository.Find(model.AssetId.Value);
            if (newAsset is null)
                return NotFound<Asset>(model.AssetId.Value);

            trade.AssetId = newAsset.Id;
            trade.Asset = newAsset;
        }

        if (model.ProfileId.HasValue && model.ProfileId.Value != trade.ProfileId)
        {
            var newProfile = await profileRepository.Find(model.ProfileId.Value);
            if (newProfile is null)
                return NotFound<Profile>(model.ProfileId.Value);

            trade.ProfileId = newProfile.Id;
            trade.Profile = newProfile;
        }

        // ReSharper disable once InvertIf
        if (model.CurrencyId.HasValue && model.CurrencyId.Value != trade.CurrencyId)
        {
            var newCurrency = await currencyRepository.Find(model.CurrencyId.Value);
            if (newCurrency is null)
                return NotFound<Currency>(model.CurrencyId.Value);

            trade.CurrencyId = newCurrency.Id;
            trade.Currency = newCurrency;
        }

        return Completed();
    }

    private static OneOf<Completed, Conflict> UpdateTradeProperties(Trade trade, UpdateTradeRequestModel model)
    {
        var updateOpenedDate = model.Opened.HasValue && model.Opened.Value.UtcDateTime != trade.Opened;
        var isFinishedBeforeOpened = updateOpenedDate && trade.Finished.HasValue &&
                                   trade.Finished.Value < model.Opened!.Value.UtcDateTime;

        if (isFinishedBeforeOpened)
            return Conflict(trade.Id, SimpleTradingStrings.FinishedBeforeOpened);

        if (updateOpenedDate)
            trade.Opened = model.Opened!.Value.UtcDateTime;

        var updateSize = model.Size.HasValue && model.Size.Value != trade.Size;
        if (updateSize)
            trade.Size = model.Size!.Value;

        var updateNotes = model.Notes.IsT0;
        if (updateNotes)
            trade.Notes = model.Notes.AsT0;

        return Completed();
    }

    private static bool UpdatePositionPrices(Trade trade, UpdateTradeRequestModel model)
    {
        var positionPrices = model.EntryPrice.HasValue && model.EntryPrice.Value != trade.PositionPrices.Entry
            // create a copy of the current prices to compare it later with the original
            // by doing so you can easily detect changes
            ? trade.PositionPrices with {Entry = model.EntryPrice.Value}
            : trade.PositionPrices with { };

        if (model.ExitPrice.IsT0 && model.ExitPrice.AsT0 != trade.PositionPrices.Exit)
            positionPrices.Exit = model.ExitPrice.AsT0;

        if (model.StopLoss.IsT0 && model.StopLoss.AsT0 != trade.PositionPrices.StopLoss)
            positionPrices.StopLoss = model.StopLoss.AsT0;

        if (model.TakeProfit.IsT0 && model.TakeProfit.AsT0 != trade.PositionPrices.TakeProfit)
            positionPrices.TakeProfit = model.TakeProfit.AsT0;

        if (trade.PositionPrices == positionPrices)
            return false;

        trade.PositionPrices = positionPrices;
        return true;
    }

    private OneOf<Completed<FinishTradeResult>, NothingToFinish, Conflict> FinishTrade(Trade trade,
        UpdateTradeRequestModel model,
        bool positionPricesHaveChanged)
    {
        if (!trade.IsFinished)
            return new NothingToFinish(trade);

        var profitLossHasChanged = model.ProfitLoss.HasValue && model.ProfitLoss.Value != trade.ProfitLoss;
        var finishedHasChanged = model.Finished.HasValue && model.Finished.Value.UtcDateTime != trade.Finished;
        var manuallyEnteredResultIsSpecified = model.ManuallyEnteredResult.IsT0;

        var nothingHasChanged =
            !positionPricesHaveChanged &&
            !profitLossHasChanged &&
            !finishedHasChanged &&
            !manuallyEnteredResultIsSpecified;

        if (nothingHasChanged)
            return new NothingToFinish(trade);

        var finishedDate = model.Finished?.UtcDateTime ?? trade.Finished!.Value;
        var profitLoss = model.ProfitLoss ?? trade.ProfitLoss!.Value;

        var finishTradeConfiguration = new FinishTradeConfiguration(finishedDate, profitLoss, utcNow)
        {
            ManuallyEnteredResult = model.ManuallyEnteredResult
        };

        return trade.Finish(finishTradeConfiguration)
            .Match<OneOf<Completed<FinishTradeResult>,
                NothingToFinish,
                Conflict>>(
                completed => completed,
                conflict => conflict);
    }

    private record NothingToFinish(Trade Trade);
}