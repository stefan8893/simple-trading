using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Client;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.TradesController;

public class UpdateTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task A_trades_size_can_be_successfully_updated()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var trade = (TestData.Trade.Default with
        {
            Size = 5000
        }).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var response = await simpleTradingClient.UpdateTradeAsync(trade.Id, new UpdateTradeDto
        {
            Size = 50_000
        });

        // assert
        response.Should().NotBeNull();
        response.Warnings.Should().BeEmpty();
        var updatedTrade = await DbContext
            .Trades
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == trade.Id);

        updatedTrade.Should().NotBeNull();
        updatedTrade!.Size.Should().Be(50_000);
    }

    [Fact]
    public async Task An_non_existing_trade_cannot_be_updated()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var notExistingTradeId = Guid.Parse("74af4aee-9582-49ab-956a-1fd7d6f8609d");

        // act
        var act = () => simpleTradingClient.UpdateTradeAsync(notExistingTradeId, new UpdateTradeDto());

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task The_closed_date_of_a_non_closed_trade_cannot_be_updated()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var trade = TestData.Trade.Default.Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => simpleTradingClient.UpdateTradeAsync(trade.Id, new UpdateTradeDto
        {
            Closed = DateTimeOffset.Parse("2024-08-14T17:00:00")
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        exception.Which.Result.CommonErrors.Should().HaveCount(1)
            .And.Contain(x =>
                x ==
                "Die Aktualisierung von 'Bilanz' und 'Abgeschlossen' ist nur möglich, wenn ein Trade bereits abgeschlossen wurde.");
    }
}