using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.References.AddReference;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.References;

public class AddReferenceTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private IAddReference Interactor => ServiceLocator.GetRequiredService<IAddReference>();

    [Fact]
    public async Task A_reference_type_out_of_enum_range_is_not_allowed()
    {
        var trade = TestData.Trade.Default.Build();
        var referenceRequestModel =
            new AddReferenceRequestModel(trade.Id, (ReferenceType) 50, "https://example.org", "some notes");

        var response = await Interactor.Execute(referenceRequestModel);

        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Reference type' has a range of values which does not include '50'.");
    }

    [Fact]
    public async Task You_cant_add_a_reference_to_a_not_existing_trade()
    {
        var notExistingTradeId = Guid.Parse("a3e474d7-688a-46db-8f7f-2b8458490168");
        var referenceRequestModel =
            new AddReferenceRequestModel(notExistingTradeId, ReferenceType.Other, "https://example.org", "some notes");

        var response = await Interactor.Execute(referenceRequestModel);

        var notFound = response.Value.Should().BeOfType<NotFound<Trade>>();
        notFound.Which.ResourceId.Should().Be(notExistingTradeId);
    }

    [Fact]
    public async Task You_cant_add_more_than_50_reference_to_a_trade()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var references = Enumerable
            .Range(0, 50)
            .Select(x => (TestData.Reference.Default with {TradeOrId = trade}).Build())
            .ToList();

        DbContext.Trades.Add(trade);
        DbContext.References.AddRange(references);
        await DbContext.SaveChangesAsync();

        var referenceRequestModel =
            new AddReferenceRequestModel(trade.Id, ReferenceType.Other, "https://example.org", "some notes");

        // act
        var response = await Interactor.Execute(referenceRequestModel);

        // assert
        var businessError = response.Value.Should().BeOfType<BusinessError>();
        businessError.Which.ResourceId.Should().Be(trade.Id);
        businessError.Which.Reason.Should()
            .Be("You can't add more than 50 references per trade.");
    }

    [Fact]
    public async Task A_reference_gets_successfully_added()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var referenceRequestModel =
            new AddReferenceRequestModel(trade.Id, ReferenceType.Other, "https://example.org", "some notes");

        // act
        var response = await Interactor.Execute(referenceRequestModel);

        // assert
        var referenceId = response.Value.Should().BeOfType<Completed<Guid>>().Which.Data;
        var tradeWithAddedReference = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        tradeWithAddedReference.Should().NotBeNull();
        tradeWithAddedReference!.References.Should().HaveCount(1)
            .And.Contain(x => x.Id == referenceId);
    }
}