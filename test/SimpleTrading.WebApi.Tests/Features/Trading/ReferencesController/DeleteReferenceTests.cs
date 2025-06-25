using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using SimpleTrading.Client;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.ReferencesController;

public class DeleteReferenceTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task References_of_a_non_existing_trade_cannot_be_deleted()
    {
        // arrange
        var client = await CreateClient();
        var notExistingTradeId = Guid.Parse("c8856d60-c650-4ae7-99b0-af87771c1186");
        var notExistingReferenceId = Guid.Parse("d5b9a98d-b4b0-44b7-9ae2-f4aac2edfde1");

        // act
        var act = () => client.DeleteReferenceAsync(notExistingTradeId, notExistingReferenceId);

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        exception.Which.Result.Messages.Should().HaveCount(1)
            .And.Contain(x => x == "Trade nicht gefunden.");
    }

    [Fact]
    public async Task References_can_be_successfully_deleted()
    {
        // arrange
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        var reference1 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        var reference2 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        DbContext.AddRange(trade, reference1, reference2);
        await DbContext.SaveChangesAsync();

        // act
        var countOfDeletedReferences = await client.DeleteReferencesAsync(trade.Id);

        // assert
        countOfDeletedReferences.Should().Be(2);
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);

        updatedTrade.Should().NotBeNull();
        updatedTrade!.References.Should().BeEmpty();
    }
}