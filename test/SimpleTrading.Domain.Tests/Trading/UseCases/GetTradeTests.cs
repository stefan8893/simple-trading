using Autofac;
using AwesomeAssertions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.GetTrade;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class GetTradeTests : DomainTests
{
    private IGetTrade Interactor => ServiceLocator.Resolve<IGetTrade>();

    [Fact]
    public async Task A_not_existing_trade_cant_be_returned()
    {
        var notExistingTradeId = Guid.Parse("a622d632-a7ef-42fe-adfa-fcb917e65926");

        var response = await Interactor.Execute(new GetTradeRequestModel(notExistingTradeId));

        response.Value.Should().BeOfType<NotFound<Trade>>()
            .Which.ResourceId.Should().Be(notExistingTradeId);
    }

    [Fact]
    public async Task An_existing_trade_gets_returned()
    {
        var trade = TestData.Trade.Default.Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var response = await Interactor.Execute(new GetTradeRequestModel(trade.Id));

        response.Value.Should().BeOfType<TradeResponseModel>()
            .Which.Id.Should().Be(trade.Id);
    }

    [Fact]
    public async Task The_Currency_property_must_contain_the_currency_s_iso_code()
    {
        var currency = TestData.Currency.Default with {IsoCode = "EUR", Name = "Euro"};
        var trade = (TestData.Trade.Default with {CurrencyOrId = currency}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var response = await Interactor.Execute(new GetTradeRequestModel(trade.Id));

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

        var response = await Interactor.Execute(new GetTradeRequestModel(trade.Id));

        response.Value.Should().BeOfType<TradeResponseModel>()
            .Which.Asset.Should().Be(asset.Symbol);
    }
}