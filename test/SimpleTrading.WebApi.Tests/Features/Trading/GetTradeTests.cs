﻿using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SimpleTrading.Client;
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
    public async Task An_existing_trade_get_returned()
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
    public async Task References_must_be_included_in_the_response()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var trade = TestData.Trade.Default.Build();
        var referenceOne = (TestData.Reference.Default with
            {
                TradeOrId = trade,
                Link = new Uri("http://example.org"),
                Type = ReferenceType.Other,
                Notes = "Link does not point to trading view."
            })
            .Build();

        var referenceTwo = (TestData.Reference.Default with
            {
                TradeOrId = trade,
                Link = new Uri("https://www.tradingview.com/x/9MYkAogh/"),
                Type = ReferenceType.TradingView
            })
            .Build();

        DbContext.AddRange(trade, referenceOne, referenceTwo);
        await DbContext.SaveChangesAsync();

        // act
        var returnedTrade = await simpleTradingClient.GetTradeAsync(trade.Id);

        // assert
        returnedTrade.References.Should().HaveCount(2)
            .And.Contain(x => x.Id == referenceOne.Id)
            .And.Contain(x => x.Id == referenceTwo.Id);
    }
}