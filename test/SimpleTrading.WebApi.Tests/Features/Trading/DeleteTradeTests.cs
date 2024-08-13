using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Client;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading;

public class DeleteTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task When_deleting_a_non_existing_trade_the_api_returns_success_in_order_to_be_idempotent()
    {
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);
        var notExistingTradeId = Guid.Parse("a47e07af-e0ae-49d0-8e1f-d0748f989c80");

        var response = await simpleTradingClient.DeleteTradeAsync(notExistingTradeId);

        response.Should().BeOfType<SuccessResponse>();
    }

    [Fact]
    public async Task A_trade_can_be_successfully_deleted()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var trade = TestData.Trade.Default.Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var response = await simpleTradingClient.DeleteTradeAsync(trade.Id);

        // assert
        response.Should().BeOfType<SuccessResponse>();
        var storedTrade = await DbContext.Trades.AsNoTracking().FirstOrDefaultAsync(x => x.Id == trade.Id);
        storedTrade.Should().BeNull();
    }
}