using Microsoft.AspNetCore.Http;
using SimpleTrading.Client;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.ReferencesController;

public class UpdateReferenceTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task A_reference_can_be_successfully_updated()
    {
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        var reference = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        DbContext.AddRange(trade, reference);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await client.UpdateReferenceAsync(trade.Id, reference.Id, new UpdateReferenceDto
        {
            Link = "https://www.tradingview.com/x/RRJnEMaI/"
        }, TestContext.Current.CancellationToken);

        var updatedReference = await DbContextSingleOrDefault<Reference>(x => x.Id == reference.Id);
        Assert.NotNull(updatedReference);
        Assert.Equal("https://www.tradingview.com/x/RRJnEMaI/", updatedReference.Link.AbsoluteUri);
    }
    
    [Fact]
    public async Task References_of_a_non_existing_trade_cannot_be_updated()
    {
        var client = await CreateClient();

        var notExistingTradeId = Guid.Parse("c2e4edf0-8fa9-492b-9f9f-be883c7ad3ed");
        var reference = TestData.Reference.Default.Build();

        DbContext.References.Add(reference);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task Act()
        {
            return client.UpdateReferenceAsync(notExistingTradeId, reference.Id,
                new UpdateReferenceDto
                {
                    Link = "https://example.org"
                });
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.Equal("Trade nicht gefunden.", exception.Result.Title);
        Assert.Equal($"Trade mit der ID '{notExistingTradeId}' nicht gefunden.", exception.Result.Detail);
    }

    [Fact]
    public async Task A_non_existing_reference_cannot_be_updated()
    {
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        var notExistingReference = Guid.Parse("cab4f9ae-c690-4875-8560-7121e73e1183");

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task Act()
        {
            return client.UpdateReferenceAsync(trade.Id, notExistingReference, new UpdateReferenceDto
            {
                Link = "https://example.org"
            });
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.Equal("Referenz nicht gefunden.", exception.Result.Title);
        Assert.Equal($"Referenz mit der ID '{notExistingReference}' nicht gefunden.", exception.Result.Detail);
    }
}