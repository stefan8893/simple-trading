using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Client;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.ReferencesController;

public class DeletesReferenceTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task References_of_a_non_existing_trade_cannot_be_deleted()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);
        var notExistingTradeId = Guid.Parse("c8856d60-c650-4ae7-99b0-af87771c1186");

        // act
        var act = () => simpleTradingClient.DeleteReferencesAsync(notExistingTradeId);

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        exception.Which.Result.CommonErrors.Should().HaveCount(1)
            .And.Contain(x => x == "Trade nicht gefunden.");
    }

    [Fact]
    public async Task A_reference_can_be_successfully_deleted()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var trade = TestData.Trade.Default.Build();
        var reference1 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        var reference2 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        DbContext.AddRange(trade, reference1, reference2);
        await DbContext.SaveChangesAsync();

        // act
        var response = await simpleTradingClient.DeleteReferenceAsync(trade.Id, reference1.Id);

        // assert
        response.Should().NotBeNull();
        response.Warnings.Should().BeEmpty();
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);

        updatedTrade.Should().NotBeNull();
        updatedTrade!.References.Should().HaveCount(1)
            .And.Contain(x => x.Id == reference2.Id);
    }
}