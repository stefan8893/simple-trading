using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.References.DeleteReferences;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.References;

public class DeleteReferencesTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private IDeleteReferences CreateInteractor()
    {
        return ServiceLocator.GetRequiredService<IDeleteReferences>();
    }

    [Fact]
    public async Task References_can_be_successfully_deleted()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var reference1 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        var reference2 = (TestData.Reference.Default with {TradeOrId = trade}).Build();

        DbContext.AddRange(trade, reference1, reference2);
        await DbContext.SaveChangesAsync();

        // act
        var response = await CreateInteractor().Execute(new DeleteReferencesRequestModel(trade.Id));

        // assert
        response.Value.Should().BeOfType<Completed<ushort>>().Which.Data.Should().Be(2);
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        updatedTrade.Should().NotBeNull();
        updatedTrade!.References.Should().BeEmpty();
    }

    [Fact]
    public async Task References_of_a_non_existing_trade_cannot_be_deleted()
    {
        var notExistingTradeId = Guid.Parse("f4d1c2c8-28c6-49b7-b6a5-78fd43412008");

        var response = await CreateInteractor().Execute(new DeleteReferencesRequestModel(notExistingTradeId));

        var notFound = response.Value.Should().BeOfType<NotFound<Trade>>();
        notFound.Which.ResourceId.Should().Be(notExistingTradeId);
    }
}