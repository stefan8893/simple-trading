using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SimpleTrading.Client;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading;

public class GetTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task A_missing_trade_cant_be_requested()
    {
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);
        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        var act = () => simpleTradingClient.GetTradeAsync(notExistingTradeId);

        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task An_existing_trade_gets_returned()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);
        var trade = TestData.Trade.Default.Build();

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var returnedTrade = await simpleTradingClient.GetTradeAsync(trade.Id);

        // assert
        returnedTrade.Id.Should().Be(trade.Id);
    }

    [Fact]
    public async Task An_trades_opened_date_gets_converted_to_the_users_timezone()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);
        var trade = (TestData.Trade.Default with
            {
                Opened = DateTime.Parse("2024-08-05T14:00:00").ToUtcKind()
            })
            .Build();

        var userSettings = await DbContext.GetUserSettings();
        userSettings.TimeZone = "America/New_York";

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var returnedTrade = await simpleTradingClient.GetTradeAsync(trade.Id);

        // assert
        var expectedOpenedDate = DateTimeOffset.Parse("2024-08-05T10:00:00-04:00");
        returnedTrade.Opened.Should().Be(expectedOpenedDate);
    }

    [Fact]
    public async Task References_must_be_included_in_the_response()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var trade = TestData.Trade.Default.Build();
        var exampleReference = (TestData.Reference.Default with
            {
                TradeOrId = trade,
                Link = new Uri("https://example.org"),
                Type = ReferenceType.Other,
                Notes = "Link does not point to trading view."
            })
            .Build();

        var tradingViewReference = (TestData.Reference.Default with
            {
                TradeOrId = trade,
                Link = new Uri("https://www.tradingview.com/x/9MYkAogh/"),
                Type = ReferenceType.TradingView
            })
            .Build();

        DbContext.AddRange(trade, exampleReference, tradingViewReference);
        await DbContext.SaveChangesAsync();

        // act
        var returnedTrade = await simpleTradingClient.GetTradeAsync(trade.Id);

        // assert
        returnedTrade.References.Should().HaveCount(2)
            .And.Contain(x => x.Id == exampleReference.Id)
            .And.Contain(x => x.Id == tradingViewReference.Id);
    }
}