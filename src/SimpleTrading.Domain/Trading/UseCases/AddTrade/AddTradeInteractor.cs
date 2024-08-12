using FluentValidation;
using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

using AddTradeResponse =
    OneOf<Completed<AddTradeResponseModel>,
        CompletedWithWarnings<AddTradeResponseModel>,
        BadInput,
        NotFound,
        BusinessError>;

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

        var potentiallyClosedTrade = TryCloseTrade(trade, model);

        if (potentiallyClosedTrade.Value is BusinessError businessError)
            return businessError;

        dbContext.Add(trade);
        await dbContext.SaveChangesAsync();

        return potentiallyClosedTrade.Match<AddTradeResponse>(
            x => Completed(CreateResponseModel(x.Data)),
            x => CompletedWithWarnings(CreateResponseModel(x.Data), x.Warnings),
            x => Completed(CreateResponseModel(x.Trade)),
            x => x);

        AddTradeResponseModel CreateResponseModel(Trade t)
        {
            return new AddTradeResponseModel(t.Id, t.Result?.ToResultModel(), t.Result?.Performance);
        }
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

    private OneOf<Completed<Trade>, CompletedWithWarnings<Trade>, NothingToClose, BusinessError> TryCloseTrade(
        Trade trade,
        AddTradeRequestModel model)
    {
        return model switch
        {
            {Balance: not null, Closed: not null} => Map(Close()),
            {Balance: null, Closed: null} => new NothingToClose(trade),
            _ => BusinessError(trade.Id, SimpleTradingStrings.ClosedTradeNeedsClosedAndBalance)
        };

        OneOf<Completed<Trade>, CompletedWithWarnings<Trade>, BusinessError> Close()
        {
            var result = trade.Close(new Trade.CloseTradeDto(model.Closed!.Value, model.Balance!.Value,
                utcNow)
            {
                ExitPrice = model.ExitPrice,
                Result = model.Result
            });

            return result
                .MapT0(_ => Completed(trade))
                .MapT1(x => CompletedWithWarnings(trade, x.Warnings));
        }

        OneOf<Completed<Trade>, CompletedWithWarnings<Trade>, NothingToClose, BusinessError> Map(
            OneOf<Completed<Trade>, CompletedWithWarnings<Trade>, BusinessError> closeTradeResult)
        {
            return closeTradeResult
                .Match<OneOf<Completed<Trade>, CompletedWithWarnings<Trade>, NothingToClose, BusinessError>>(
                    _ => Completed(trade),
                    x => CompletedWithWarnings(trade, x.Warnings),
                    x => x);
        }
    }

    private record NothingToClose(Trade Trade);
}