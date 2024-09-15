using FluentValidation;
using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.UpdateTrade;

using UpdateTradeResponse =
    OneOf<Completed,
        BadInput,
        NotFound,
        BusinessError>;

public class UpdateTradeInteractor(
    IValidator<UpdateTradeRequestModel> validator,
    ITradeRepository tradeRepository,
    UowCommit uowCommit,
    UtcNow utcNow)
    : BaseInteractor, IUpdateTrade
{
    public async Task<UpdateTradeResponse> Execute(UpdateTradeRequestModel model)
    {
        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
            return BadInput(validationResult);

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
            return updateEntitiesResult.Match<UpdateTradeResponse>(_ => Completed(), x => x);

        var updatePropertiesResult = UpdateTradeProperties(trade, model);
        if (updatePropertiesResult.Value is BusinessError propertiesBusinessError)
            return propertiesBusinessError;

        var hasChanges = UpdatePositionPrices(trade, model);
        var closeTradeResult = CloseTrade(trade, model, hasChanges);

        if (closeTradeResult.Value is BusinessError closeTradeBusinessError)
            return closeTradeBusinessError;

        await uowCommit();
        return closeTradeResult
            .Match<UpdateTradeResponse>(x => Completed(x.Warnings),
                _ => Completed(),
                x => x);
    }

    private async Task<OneOf<Completed, NotFound>> UpdateEntities(Trade trade,
        UpdateTradeRequestModel model)
    {
        if (model.AssetId.HasValue && model.AssetId.Value != trade.AssetId)
        {
            var newAsset = await tradeRepository.FindAsset(model.AssetId.Value);
            if (newAsset is null)
                return NotFound<Asset>(model.AssetId.Value);

            trade.AssetId = newAsset.Id;
            trade.Asset = newAsset;
        }

        if (model.ProfileId.HasValue && model.ProfileId.Value != trade.ProfileId)
        {
            var newProfile = await tradeRepository.FindProfile(model.ProfileId.Value);
            if (newProfile is null)
                return NotFound<Profile>(model.ProfileId.Value);

            trade.ProfileId = newProfile.Id;
            trade.Profile = newProfile;
        }

        // ReSharper disable once InvertIf
        if (model.CurrencyId.HasValue && model.CurrencyId.Value != trade.CurrencyId)
        {
            var newCurrency = await tradeRepository.FindCurrency(model.CurrencyId.Value);
            if (newCurrency is null)
                return NotFound<Currency>(model.CurrencyId.Value);

            trade.CurrencyId = newCurrency.Id;
            trade.Currency = newCurrency;
        }

        return Completed();
    }

    private static OneOf<Completed, BusinessError> UpdateTradeProperties(Trade trade, UpdateTradeRequestModel model)
    {
        var updateOpenedDate = model.Opened.HasValue && model.Opened.Value.UtcDateTime != trade.Opened;
        var isClosedBeforeOpened = updateOpenedDate && trade.Closed.HasValue &&
                                   trade.Closed.Value < model.Opened!.Value.UtcDateTime;

        if (isClosedBeforeOpened)
            return BusinessError(trade.Id, SimpleTradingStrings.ClosedBeforeOpened);

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

    private OneOf<Completed, NothingToClose, BusinessError> CloseTrade(Trade trade, UpdateTradeRequestModel model,
        bool hasPositionPricesChanges)
    {
        var balanceHasChanged = model.Balance.HasValue && model.Balance.Value != trade.Balance;
        var closedHasChanged = model.Closed.HasValue && model.Closed.Value.UtcDateTime != trade.Closed;
        var resultHasChanged = model.Result.IsT0 && model.Result.AsT0?.ToString() != trade.Result?.Name;
        var shouldResetResult = resultHasChanged && model.Result.AsT0 is null;

        if (!trade.IsClosed && (balanceHasChanged || closedHasChanged))
            return new BusinessError(trade.Id,
                SimpleTradingStrings.BalanceAndClosedUpdatesAreOnlyPossibleForClosedTrades);

        if (resultHasChanged && shouldResetResult)
            trade.ResetManuallyEnteredResult(utcNow);

        // ReSharper disable once InvertIf
        if (trade.IsClosed &&
            (hasPositionPricesChanges || balanceHasChanged || closedHasChanged || resultHasChanged))
        {
            var closedDate = model.Closed?.UtcDateTime ?? trade.Closed!.Value;
            var balance = model.Balance ?? trade.Balance!.Value;
            var result = model.Result.IsT0 ? model.Result.AsT0 : null;

            return trade.Close(new Trade.CloseTradeDto(closedDate, balance, utcNow) {Result = result})
                .Match<OneOf<Completed, NothingToClose, BusinessError>>(x => x, x => x);
        }

        return new NothingToClose();
    }

    private record NothingToClose;
}