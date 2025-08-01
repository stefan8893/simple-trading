using Autofac;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.References.GetReferences;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.References;

public class GetReferencesTests : DomainTests
{
    private IGetReferences Interactor => ServiceLocator.Resolve<IGetReferences>();

    [Fact]
    public async Task A_non_existing_references_cant_be_returned()
    {
        var notExistingTradeId = Guid.Parse("a622d632-a7ef-42fe-adfa-fcb917e65926");

        var response = await Interactor
            .Execute(new GetReferencesRequestModel(notExistingTradeId));

        var notFound = Assert.IsType<NotFound<Trade>>(response.Value);
        Assert.Equal(notExistingTradeId, notFound.ResourceId);
    }

    [Fact]
    public async Task A_trades_references_will_be_returned()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var reference1 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        var reference2 = (TestData.Reference.Default with {TradeOrId = trade}).Build();

        DbContext.AddRange(trade, reference1, reference2);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor
            .Execute(new GetReferencesRequestModel(trade.Id));

        // assert
        var references = Assert.IsType<IReadOnlyList<ReferenceResponseModel>>(response.Value, exactMatch: false);
        Assert.Equal(2, references.Count);
        Assert.Contains(references, x => x.Id == reference1.Id);
        Assert.Contains(references, x => x.Id == reference2.Id);
    }
}