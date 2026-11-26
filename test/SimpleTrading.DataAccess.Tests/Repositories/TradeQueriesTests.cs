using Autofac;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.DataAccess.Tests.Repositories;

public class TradeQueriesTests : RepositoryTests
{
    private ITradeQueries TradeQueries => ServiceLocator.Resolve<ITradeQueries>();

    [Fact]
    public async Task Returns_not_found_if_the_trade_does_not_exist()
    {
        var notExistingTradeId = Guid.Parse("a622d632-a7ef-42fe-adfa-fcb917e65926");

        var tradeModel = await TradeQueries.FindTradeModel(notExistingTradeId);

        Assert.Null(tradeModel);
    }

    [Fact]
    public async Task An_existing_trade_gets_returned()
    {
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        var currency = TestData.Currency.Default.Build();
        var trade = (TestData.Trade.Default with
        {
            ProfileOrId = profile,
            AssetOrId = asset,
            CurrencyOrId = currency
        }).Build();
        DbContext.AddRange(trade, profile, asset, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await TradeQueries.FindTradeModel(trade.Id);

        var result = Assert.IsType<TradeResponseModel>(response);
        Assert.Equal(trade.Id, result.Id);
    }

    [Fact]
    public async Task The_Currency_property_must_contain_the_currency_s_iso_code()
    {
        await DbContext.Currencies.ExecuteDeleteAsync(TestContext.Current.CancellationToken);

        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        var currency = (TestData.Currency.Default with { IsoCode = "EUR", Name = "Euro" }).Build();
        var trade = (TestData.Trade.Default with
        {
            ProfileOrId = profile,
            AssetOrId = asset,
            CurrencyOrId = currency
        }).Build();
        DbContext.AddRange(trade, profile, asset, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await TradeQueries.FindTradeModel(trade.Id);

        var result = Assert.IsType<TradeResponseModel>(response);
        Assert.Equal(currency.IsoCode, result.Currency);
    }

    [Fact]
    public async Task The_Asset_property_must_contain_the_assets_symbol()
    {
        var profile = TestData.Profile.Default.Build();
        var asset = (TestData.Asset.Default with { Symbol = "NDQ", Name = "US 100" }).Build();
        var currency = TestData.Currency.Default.Build();
        var trade = (TestData.Trade.Default with
        {
            ProfileOrId = profile,
            AssetOrId = asset,
            CurrencyOrId = currency
        }).Build();
        DbContext.AddRange(trade, profile, asset, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await TradeQueries.FindTradeModel(trade.Id);

        var result = Assert.IsType<TradeResponseModel>(response);
        Assert.Equal(asset.Symbol, result.Asset);
    }
}