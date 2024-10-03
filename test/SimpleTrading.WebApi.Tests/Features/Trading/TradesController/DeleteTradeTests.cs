using FluentAssertions;
using SimpleTrading.Client;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.TradesController;

public class DeleteTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task When_deleting_a_non_existing_trade_the_api_returns_success_in_order_to_be_idempotent()
    {
        var client = await CreateClient();
        var notExistingTradeId = Guid.Parse("a47e07af-e0ae-49d0-8e1f-d0748f989c80");

        var act = () => client.DeleteTradeAsync(notExistingTradeId);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task A_trade_can_be_successfully_deleted()
    {
        // arrange
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => client.DeleteTradeAsync(trade.Id);

        // assert
        await act.Should().NotThrowAsync();
        var storedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        storedTrade.Should().BeNull();
    }
}