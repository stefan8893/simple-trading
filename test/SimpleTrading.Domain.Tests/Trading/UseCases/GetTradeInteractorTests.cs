using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.GetTrade;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class GetTradeInteractorTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private IGetTrade CreateInteractor()
    {
        return ServiceLocator.GetRequiredService<IGetTrade>();
    }

    [Fact]
    public async Task A_not_existing_trade_cant_be_returned()
    {
        var notExistingAssetId = Guid.Parse("a622d632-a7ef-42fe-adfa-fcb917e65926");

        var response = await CreateInteractor().Execute(notExistingAssetId);

        response.Value.Should().BeOfType<NotFound<Trade>>()
            .Which.ResourceId.Should().Be(notExistingAssetId);
    }

    [Fact]
    public async Task An_existing_trade_gets_returned()
    {
        var trade = TestData.Trade.Default.Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var response = await CreateInteractor().Execute(trade.Id);

        response.Value.Should().BeOfType<TradeResponseModel>()
            .Which.Id.Should().Be(trade.Id);
    }

    [Fact]
    public async Task The_Currency_property_must_contain_the_currencys_iso_code()
    {
        var currency = TestData.Currency.Default with {IsoCode = "EUR", Name = "Euro"};
        var trade = (TestData.Trade.Default with {CurrencyOrId = currency}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var response = await CreateInteractor().Execute(trade.Id);

        response.Value.Should().BeOfType<TradeResponseModel>()
            .Which.Currency.Should().Be(currency.IsoCode);
    }

    [Fact]
    public async Task The_Asset_property_must_contain_the_assets_symbol()
    {
        var asset = TestData.Asset.Default with {Symbol = "NDQ", Name = "US 100"};
        var trade = (TestData.Trade.Default with {AssetOrId = asset}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var response = await CreateInteractor().Execute(trade.Id);

        response.Value.Should().BeOfType<TradeResponseModel>()
            .Which.Asset.Should().Be(asset.Symbol);
    }
}