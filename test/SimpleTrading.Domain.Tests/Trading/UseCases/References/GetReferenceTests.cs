using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.References.GetReference;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.References;

public class GetReferenceTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private IGetReference Interactor => ServiceLocator.GetRequiredService<IGetReference>();

    [Fact]
    public async Task A_not_existing_reference_cant_be_returned()
    {
        var notExistingTradeId = Guid.Parse("a622d632-a7ef-42fe-adfa-fcb917e65926");
        var notExistingReferenceId = Guid.Parse("5fb9a049-a309-4617-981e-49de0e86bc86");

        var response = await Interactor
            .Execute(new GetReferenceRequestModel(notExistingTradeId, notExistingReferenceId));

        response.Value.Should().BeOfType<NotFound<Trade>>()
            .Which.ResourceId.Should().Be(notExistingTradeId);
    }

    [Fact]
    public async Task A_trades_reference_gets_returned()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var reference1 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        var reference2 = (TestData.Reference.Default with {TradeOrId = trade}).Build();

        DbContext.AddRange(trade, reference1, reference2);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor
            .Execute(new GetReferenceRequestModel(trade.Id, reference1.Id));

        // assert
        var responseModel = response.Value.Should().BeOfType<ReferenceModel>();
        responseModel.Which.Id.Should().Be(reference1.Id);
        responseModel.Which.Link.Should().Be(reference1.Link.AbsoluteUri);
        responseModel.Which.Type.Should().Be(reference1.Type);
        responseModel.Which.Notes.Should().Be(reference1.Notes);
    }
}