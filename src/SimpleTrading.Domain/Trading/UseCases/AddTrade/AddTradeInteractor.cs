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

        return await AddTrade(model, asset, profile, currency);
    }

    private async Task<AddTradeResponse> AddTrade(AddTradeRequestModel model, Asset asset, Profile profile,
        Currency currency)
    {
        var trade = CreateTrade(model, asset, profile, currency);

        var userSettings = await dbContext.GetUserSettings();
        var closedTrade = CloseTrade(trade, model, userSettings.TimeZone);

        if (closedTrade.Value is BusinessError businessError)
            return businessError;

        dbContext.Add(trade);
        await dbContext.SaveChangesAsync();

        return Completed(trade.Id);
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
            Opened = model.Opened.ToUtcKind(),
            CurrencyId = currency.Id,
            Currency = currency,
            PositionPrices = new PositionPrices
            {
                Entry = model.EntryPrice,
                StopLoss = model.StopLoss,
                TakeProfit = model.TakeProfit
            },
            Notes = model.Notes,
            CreatedAt = utcNow()
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
                CreatedAt = utcNow()
            });

        return newTrade;
    }

    private OneOf<Completed<Trade>, BusinessError> CloseTrade(Trade trade, AddTradeRequestModel model, string timeZone)
    {
        return model switch
        {
            {Balance: not null, Result: not null, Closed: not null, ExitPrice: not null} => Close(),
            {Balance: null, Result: null, Closed: null, ExitPrice: null} => Completed(trade),
            _ => BusinessError(trade.Id, SimpleTradingStrings.ClosedTradeNeedsClosedBalanceAndResult)
        };

        OneOf<Completed<Trade>, BusinessError> Close()
        {
            var result = trade.Close(new Trade.CloseTradeDto(model.Result!.Value, model.Balance!.Value,
                model.ExitPrice!.Value, model.Closed!.Value, utcNow, timeZone));

            return result.MapT0(x => Completed(trade));
        }
    }
}