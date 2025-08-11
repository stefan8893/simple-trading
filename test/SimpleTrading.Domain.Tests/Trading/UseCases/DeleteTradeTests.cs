using Autofac;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.DeleteTrade;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class DeleteTradeTests : DomainTests
{
    private IDeleteTrade Interactor => ServiceLocator.Resolve<IDeleteTrade>();

    [Fact]
    public async Task A_not_existing_trade_cannot_be_deleted()
    {
        var notExistingTradeId = Guid.Parse("a47e07af-e0ae-49d0-8e1f-d0748f989c80");

        var response = await Interactor.Execute(notExistingTradeId, TestContext.Current.CancellationToken);

        var notFound = Assert.IsType<NotFound<Trade>>(response.Value);
        Assert.Equal(notExistingTradeId, notFound.ResourceId);
    }

    [Fact]
    public async Task A_trade_can_be_successfully_deleted()
    {
        var trade = TestData.Trade.Default.Build();

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await Interactor.Execute(trade.Id, TestContext.Current.CancellationToken);

        Assert.IsType<Completed>(response.Value);
        var storedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        Assert.Null(storedTrade);
    }
}