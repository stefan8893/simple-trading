using Autofac;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.References.GetReference;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.References;

public class GetReferenceTests : DomainTests
{
    private IGetReference Interactor => ServiceLocator.Resolve<IGetReference>();

    [Fact]
    public async Task A_not_existing_reference_cant_be_returned()
    {
        var notExistingTradeId = Guid.Parse("a622d632-a7ef-42fe-adfa-fcb917e65926");
        var notExistingReferenceId = Guid.Parse("5fb9a049-a309-4617-981e-49de0e86bc86");

        var response = await Interactor
            .Execute(new GetReferenceRequestModel(notExistingTradeId, notExistingReferenceId));

        var notFound = Assert.IsType<NotFound<Trade>>(response.Value);
        Assert.Equal(notExistingTradeId, notFound.ResourceId);
    }

    [Fact]
    public async Task A_trades_reference_gets_returned()
    {
        var trade = TestData.Trade.Default.Build();
        var reference1 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        var reference2 = (TestData.Reference.Default with {TradeOrId = trade}).Build();

        DbContext.AddRange(trade, reference1, reference2);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await Interactor
            .Execute(new GetReferenceRequestModel(trade.Id, reference1.Id));

        var referenceModel = Assert.IsType<ReferenceResponseModel>(response.Value);
        Assert.Equal(reference1.Id, referenceModel.Id);
        Assert.Equal(reference1.Link.AbsoluteUri, referenceModel.Link);
        Assert.Equal(reference1.Type, referenceModel.Type);
        Assert.Equal(reference1.Notes, referenceModel.Notes);
    }
}