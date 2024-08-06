using FluentValidation;
using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

using AddTradeResponse = OneOf<Completed<Guid>, BadInput, NotFound, BusinessError>;

public class AddTradeInteractor(IValidator<AddTradeRequestModel> validator, TradingDbContext dbContext, UtcNow utcNow)
    : BaseInteractor, IAddTrade
{
    public async Task<AddTradeResponse> Execute(AddTradeRequestModel model)
    {
        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
            return BadInput(validationResult);

        var asset = await dbContext.Assets.FindAsync(model.AssetId);
        if (asset is null)
            return NotFound<Asset>(model.AssetId);

        var profile = await dbContext.Profiles.FindAsync(model.ProfileId);
        if (profile is null)
            return NotFound<Profile>(model.ProfileId);

        var currency = await dbContext.Currencies.FindAsync(model.CurrencyId);
        if (currency is null)
            return NotFound<Currency>(model.CurrencyId);

        var trade = CreateTrade(model, asset, profile, currency);

        var userSettings = await dbContext.GetUserSettings();
        var result = FinishTrade(trade, model, userSettings.TimeZone);

        if (result.Value is Completed<Trade>)
        {
            dbContext.Add(trade);
            await dbContext.SaveChangesAsync();
        }

        return result.Match<AddTradeResponse>(
            x => Completed(x.Data.Id),
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
            OpenedAt = model.OpenedAt.ToUtcKind(),
            CurrencyId = currency.Id,
            Currency = currency,
            PositionPrices = new PositionPrices
            {
                Entry = model.EntryPrice,
                StopLoss = model.StopLoss,
                TakeProfit = model.TakeProfit
            },
            Notes = model.Notes
        };

        foreach (var x in model.References)
            newTrade.References.Add(new Reference
            {
                Id = Guid.NewGuid(),
                TradeId = newTrade.Id,
                Trade = newTrade,
                Link = new Uri(x.Link),
                Type = x.Type,
                CreatedAt = utcNow()
            });

        return newTrade;
    }

    private OneOf<Completed<Trade>, BusinessError> FinishTrade(Trade trade, AddTradeRequestModel model, string timeZone)
    {
        return model switch
        {
            {Balance: not null, Result: not null, FinishedAt: not null} => Finish(),
            {Balance: null, Result: null, FinishedAt: null} => Completed(trade),
            _ => BusinessError(trade.Id, SimpleTradingStrings.FinishedTradeNeedsFinishedBalanceAndResult)
        };

        OneOf<Completed<Trade>, BusinessError> Finish()
        {
            var result = trade.Finish(new Trade.FinishTradeDto(model.Result!.Value, model.Balance!.Value,
                model.FinishedAt!.Value, utcNow, timeZone));

            return result.MapT0(x => Completed(trade));
        }
    }
}