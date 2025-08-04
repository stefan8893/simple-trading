using Microsoft.AspNetCore.Http;
using SimpleTrading.Client;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.ReferencesController;

public class AddReferenceTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task A_reference_can_be_successfully_added()
    {
        // arrange
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        DbContext.AddRange(trade);
        await DbContext.SaveChangesAsync();

        // act
        var idOfAddedReference = await client.AddReferenceAsync(trade.Id, new AddReferenceDto
        {
            Type = NullableOfReferenceTypeDto.Other,
            Link = "https://example.org"
        });

        // assert
        var newlyAddedReference = await DbContextSingleOrDefault<Reference>(x => x.Id == idOfAddedReference);
        Assert.NotNull(newlyAddedReference);
    }

    [Fact]
    public async Task A_reference_with_an_invalid_uri_cannot_be_added()
    {
        // arrange
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        DbContext.AddRange(trade);
        await DbContext.SaveChangesAsync();

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<Guid> Act()
        {
            return client.AddReferenceAsync(trade.Id, new AddReferenceDto
            {
                Type = NullableOfReferenceTypeDto.Other,
                Link = "invalid-uri"
            });
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<FieldErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("link", error.Identifier);
        Assert.Equal("Ungültiger Link.", Assert.Single(error.Messages));
    }

    [Fact]
    public async Task References_cannot_be_added_to_non_existing_trades()
    {
        // arrange
        var client = await CreateClient();
        var notExistingTradeId = Guid.Parse("c2e4edf0-8fa9-492b-9f9f-be883c7ad3ed");

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<Guid> Act()
        {
            return client.AddReferenceAsync(notExistingTradeId, new AddReferenceDto
            {
                Type = NullableOfReferenceTypeDto.Other,
                Link = "https://example.org"
            });
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.Equal("Trade nicht gefunden.", Assert.Single(exception.Result.Messages));
    }
}