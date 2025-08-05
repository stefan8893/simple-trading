using Microsoft.AspNetCore.Http;
using SimpleTrading.Client;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.ReferencesController;

public class GetReferenceTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task References_of_a_non_existing_trade_cannot_be_found()
    {
        // arrange
        var client = await CreateClient();

        var notExistingTradeId = Guid.Parse("f1e3aed3-da10-481d-a48c-f9686bccb484");
        var notExistingReferenceId = Guid.Parse("c8856d60-c650-4ae7-99b0-af87771c1186");

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task Act()
        {
            return client.GetReferenceAsync(notExistingTradeId, notExistingReferenceId);
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.Equal("Trade nicht gefunden.", exception.Result.Detail);
    }

    [Fact]
    public async Task A_non_existing_reference_cannot_be_found()
    {
        // arrange
        var client = await CreateClient();
        var trade = TestData.Trade.Default.Build();
        var reference = (TestData.Reference.Default with {TradeOrId = trade}).Build();

        DbContext.AddRange(trade, reference);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var notExistingReferenceId = Guid.Parse("c8856d60-c650-4ae7-99b0-af87771c1186");

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task Act()
        {
            return client.GetReferenceAsync(trade.Id, notExistingReferenceId);
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.Equal("Referenz nicht gefunden.", exception.Result.Detail);
    }

    [Fact]
    public async Task An_existing_reference_gets_successfully_returned()
    {
        // arrange
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        var reference1 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        var reference2 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        DbContext.AddRange(trade, reference1, reference2);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var response = await client.GetReferenceAsync(trade.Id, reference2.Id, TestContext.Current.CancellationToken);

        // assert
        Assert.NotNull(response);
        Assert.Equal(reference2.Id, response.Id);
        Assert.Equal(reference2.Link.AbsoluteUri, response.Link);
        Assert.Equal(reference2.Notes, response.Notes);
    }
}