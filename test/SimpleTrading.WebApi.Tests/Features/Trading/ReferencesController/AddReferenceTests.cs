using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var trade = TestData.Trade.Default.Build();
        DbContext.AddRange(trade);
        await DbContext.SaveChangesAsync();

        // act
        var response = await simpleTradingClient.AddReferenceAsync(trade.Id, new AddReferenceDto
        {
            Type = ReferenceTypeDto.Other,
            Link = "https://example.org"
        });

        // assert
        response.Should().NotBeNull();
        response.Warnings.Should().BeEmpty();
        response.Data.Should().NotBeNull();
        var newlyAddedReference = await DbContextSingleOrDefault<Reference>(x => x.Id == response.Data);

        newlyAddedReference.Should().NotBeNull();
    }

    [Fact]
    public async Task A_reference_with_an_invalid_uri_cannot_be_added()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var trade = TestData.Trade.Default.Build();
        DbContext.AddRange(trade);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => simpleTradingClient.AddReferenceAsync(trade.Id, new AddReferenceDto
        {
            Type = ReferenceTypeDto.Other,
            Link = "invalid-uri"
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.FieldErrors.Should().HaveCount(1)
            .And.Contain(x => x.Messages.Single() == "Ungültiger Link.")
            .And.Contain(x => x.Identifier == "Link");
    }

    [Fact]
    public async Task References_cannot_be_added_to_non_existing_trades()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var notExistingTradeId = Guid.Parse("c2e4edf0-8fa9-492b-9f9f-be883c7ad3ed");

        // act
        var act = () => simpleTradingClient.AddReferenceAsync(notExistingTradeId, new AddReferenceDto
        {
            Type = ReferenceTypeDto.Other,
            Link = "https://example.org"
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        exception.Which.Result.CommonErrors.Should().HaveCount(1)
            .And.Contain(x => x == "Trade nicht gefunden.");
    }
}