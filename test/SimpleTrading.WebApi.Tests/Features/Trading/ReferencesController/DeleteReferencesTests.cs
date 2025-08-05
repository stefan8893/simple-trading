using Microsoft.AspNetCore.Http;
using SimpleTrading.Client;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.ReferencesController;

public class DeleteReferencesTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task References_of_a_non_existing_trade_cannot_be_deleted()
    {
        // arrange
        var client = await CreateClient();
        var notExistingTradeId = Guid.Parse("c8856d60-c650-4ae7-99b0-af87771c1186");

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task Act()
        {
            return client.DeleteReferencesAsync(notExistingTradeId);
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.Equal("Trade nicht gefunden.", exception.Result.Detail);
    }

    [Fact]
    public async Task A_reference_can_be_successfully_deleted()
    {
        // arrange
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        var reference1 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        var reference2 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        DbContext.AddRange(trade, reference1, reference2);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        await client.DeleteReferenceAsync(trade.Id, reference1.Id, TestContext.Current.CancellationToken);

        // assert
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        Assert.NotNull(updatedTrade);
        var remainingReference = Assert.Single(updatedTrade.References);
        Assert.Equal(reference2.Id, remainingReference.Id);
    }
}