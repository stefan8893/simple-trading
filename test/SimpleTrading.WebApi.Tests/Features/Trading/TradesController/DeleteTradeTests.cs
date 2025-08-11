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

        await client.DeleteTradeAsync(notExistingTradeId, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task A_trade_can_be_successfully_deleted()
    {
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await client.DeleteTradeAsync(trade.Id, TestContext.Current.CancellationToken);

        var storedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        Assert.Null(storedTrade);
    }
}