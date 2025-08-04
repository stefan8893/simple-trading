using Autofac;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.References.DeleteReferences;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.References;

public class DeleteReferencesTests : DomainTests
{
    private IDeleteReferences Interactor => ServiceLocator.Resolve<IDeleteReferences>();

    [Fact]
    public async Task References_can_be_successfully_deleted()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var reference1 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        var reference2 = (TestData.Reference.Default with {TradeOrId = trade}).Build();

        DbContext.AddRange(trade, reference1, reference2);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var response = await Interactor.Execute(new DeleteReferencesRequestModel(trade.Id));

        // assert
        var result = Assert.IsType<Completed<ushort>>(response.Value);
        Assert.Equal(2, result.Data);
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        Assert.NotNull(updatedTrade);
        Assert.Empty(updatedTrade.References);
    }

    [Fact]
    public async Task References_of_a_non_existing_trade_cannot_be_deleted()
    {
        var notExistingTradeId = Guid.Parse("f4d1c2c8-28c6-49b7-b6a5-78fd43412008");

        var response = await Interactor.Execute(new DeleteReferencesRequestModel(notExistingTradeId));

        var notFound = Assert.IsType<NotFound<Trade>>(response.Value);
        Assert.Equal(notExistingTradeId, notFound.ResourceId);
    }
}