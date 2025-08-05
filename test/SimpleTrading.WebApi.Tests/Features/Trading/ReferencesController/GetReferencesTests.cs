using Microsoft.AspNetCore.Http;
using SimpleTrading.Client;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.ReferencesController;

public class GetReferencesTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task References_of_a_non_existing_trade_cannot_be_found()
    {
        // arrange
        var client = await CreateClient();
        var notExistingTradeId = Guid.Parse("c8856d60-c650-4ae7-99b0-af87771c1186");

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task Act()
        {
            return client.GetReferencesAsync(notExistingTradeId);
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.Equal("Trade nicht gefunden.", exception.Result.Detail);
    }

    [Fact]
    public async Task Existing_references_gets_successfully_returned()
    {
        // arrange
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        var reference1 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        var reference2 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        DbContext.AddRange(trade, reference1, reference2);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var response = await client.GetReferencesAsync(trade.Id, TestContext.Current.CancellationToken);

        // assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Count);
        Assert.Contains(response, x => x.Id == reference1.Id);
        Assert.Contains(response, x => x.Id == reference2.Id);
    }
}