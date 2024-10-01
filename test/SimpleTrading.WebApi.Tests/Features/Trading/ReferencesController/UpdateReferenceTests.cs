using FluentAssertions;
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
        // arrange
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        var reference = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        DbContext.AddRange(trade, reference);
        await DbContext.SaveChangesAsync();

        // act
        var response = await client.UpdateReferenceAsync(trade.Id, reference.Id, new UpdateReferenceDto
        {
            Type = ReferenceTypeDto.TradingView,
            Link = "https://www.tradingview.com/x/RRJnEMaI/"
        });

        // assert
        response.Should().NotBeNull();
        response.Warnings.Should().BeEmpty();
        var updatedReference = await DbContextSingleOrDefault<Reference>(x => x.Id == reference.Id);

        updatedReference.Should().NotBeNull();
        updatedReference!.Type.Should().Be(ReferenceType.TradingView);
        updatedReference.Link.AbsoluteUri.Should().Be("https://www.tradingview.com/x/RRJnEMaI/");
    }

    [Fact]
    public async Task An_update_with_an_invalid_type_is_a_bad_request()
    {
        // arrange
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        var reference = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        DbContext.AddRange(trade, reference);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => client.UpdateReferenceAsync(trade.Id, reference.Id, new UpdateReferenceDto
        {
            Type = (ReferenceTypeDto) 50
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<FieldErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.Errors.Should().HaveCount(1)
            .And.Contain(x => x.Identifier == "Type")
            .And.Contain(x => x.Messages.Single() == "'Referenztyp' hat einen Wertebereich, der '50' nicht enthält.");
    }

    [Fact]
    public async Task References_of_a_non_existing_trade_cannot_be_updated()
    {
        // arrange
        var client = await CreateClient();

        var notExistingTradeId = Guid.Parse("c2e4edf0-8fa9-492b-9f9f-be883c7ad3ed");
        var reference = TestData.Reference.Default.Build();

        DbContext.References.Add(reference);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => client.UpdateReferenceAsync(notExistingTradeId, reference.Id,
            new UpdateReferenceDto
            {
                Type = ReferenceTypeDto.Other,
                Link = "https://example.org"
            });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        exception.Which.Result.Messages.Should().HaveCount(1)
            .And.Contain(x => x == "Trade nicht gefunden.");
    }

    [Fact]
    public async Task A_non_existing_reference_cannot_be_updated()
    {
        // arrange
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        var notExistingReference = Guid.Parse("cab4f9ae-c690-4875-8560-7121e73e1183");

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => client.UpdateReferenceAsync(trade.Id, notExistingReference, new UpdateReferenceDto
        {
            Type = ReferenceTypeDto.Other,
            Link = "https://example.org"
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        exception.Which.Result.Messages.Should().HaveCount(1)
            .And.Contain(x => x == "Referenz nicht gefunden.");
    }
}