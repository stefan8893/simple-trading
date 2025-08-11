using Autofac;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.GetTrade;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class GetTradeTests : DomainTests
{
    private IGetTrade Interactor => ServiceLocator.Resolve<IGetTrade>();

    [Fact]
    public async Task Returns_not_found_if_the_trade_does_not_exist()
    {
        var notExistingTradeId = Guid.Parse("a622d632-a7ef-42fe-adfa-fcb917e65926");

        var response = await Interactor.Execute(notExistingTradeId, TestContext.Current.CancellationToken);

        var notFound = Assert.IsType<NotFound<Trade>>(response.Value);
        Assert.Equal(notExistingTradeId, notFound.ResourceId);
    }

    [Fact]
    public async Task An_existing_trade_gets_returned()
    {
        var trade = TestData.Trade.Default.Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await Interactor.Execute(trade.Id, TestContext.Current.CancellationToken);

        var result = Assert.IsType<TradeResponseModel>(response.Value);
        Assert.Equal(trade.Id, result.Id);
    }

    [Fact]
    public async Task The_Currency_property_must_contain_the_currency_s_iso_code()
    {
        await DbContext.Currencies.ExecuteDeleteAsync(TestContext.Current.CancellationToken);
        
        var currency = TestData.Currency.Default with {IsoCode = "EUR", Name = "Euro"};
        var trade = (TestData.Trade.Default with {CurrencyOrId = currency}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await Interactor.Execute(trade.Id, TestContext.Current.CancellationToken);

        var result = Assert.IsType<TradeResponseModel>(response.Value);
        Assert.Equal(currency.IsoCode, result.Currency);
    }

    [Fact]
    public async Task The_Asset_property_must_contain_the_assets_symbol()
    {
        var asset = TestData.Asset.Default with {Symbol = "NDQ", Name = "US 100"};
        var trade = (TestData.Trade.Default with {AssetOrId = asset}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await Interactor.Execute(trade.Id, TestContext.Current.CancellationToken);

        var result = Assert.IsType<TradeResponseModel>(response.Value);
        Assert.Equal(asset.Symbol, result.Asset);
    }
}