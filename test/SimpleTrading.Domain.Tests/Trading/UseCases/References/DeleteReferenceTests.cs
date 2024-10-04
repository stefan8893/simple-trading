using Autofac;
using FluentAssertions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.References.DeleteReference;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.References;

public class DeleteReferenceTests : DomainTests
{
    private IDeleteReference Interactor => ServiceLocator.Resolve<IDeleteReference>();

    [Fact]
    public async Task A_reference_can_be_successfully_deleted()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var reference1 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        var reference2 = (TestData.Reference.Default with {TradeOrId = trade}).Build();

        DbContext.AddRange(trade, reference1, reference2);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new DeleteReferenceRequestModel(trade.Id, reference1.Id));

        // assert
        response.Value.Should().BeOfType<Completed>();
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        updatedTrade.Should().NotBeNull();
        updatedTrade!.References.Should().HaveCount(1)
            .And.Contain(x => x.Id == reference2.Id);
    }

    [Fact]
    public async Task A_reference_of_a_non_existing_trade_cannot_be_deleted()
    {
        var notExistingTradeId = Guid.Parse("f4d1c2c8-28c6-49b7-b6a5-78fd43412008");
        var notExistingReferenceId = Guid.Parse("b32aef83-cd0f-430b-beaa-9f11af78f4fb");

        var response = await Interactor
            .Execute(new DeleteReferenceRequestModel(notExistingTradeId, notExistingReferenceId));

        var notFound = response.Value.Should().BeOfType<NotFound<Trade>>();
        notFound.Which.ResourceId.Should().Be(notExistingTradeId);
    }
}