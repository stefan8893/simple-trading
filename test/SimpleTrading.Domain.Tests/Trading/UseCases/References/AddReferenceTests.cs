using Autofac;
using FluentValidation.Results;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.References.AddReference;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.References;

public class AddReferenceTests : DomainTests
{
    private IAddReference Interactor => ServiceLocator.Resolve<IAddReference>();

    [Fact]
    public async Task You_cant_add_a_reference_to_a_not_existing_trade()
    {
        var notExistingTradeId = Guid.Parse("a3e474d7-688a-46db-8f7f-2b8458490168");
        var referenceRequestModel =
            new AddReferenceRequestModel(notExistingTradeId, "https://example.org", "some notes");

        var response = await Interactor.Execute(referenceRequestModel, TestContext.Current.CancellationToken);

        var notFound = Assert.IsType<NotFound<Trade>>(response.Value);
        Assert.Equal(notExistingTradeId, notFound.ResourceId);
    }

    [Fact]
    public async Task You_cant_add_more_than_5_reference_to_a_trade()
    {
        var trade = TestData.Trade.Default.Build();
        var references = Enumerable
            .Range(0, 5)
            .Select(_ => (TestData.Reference.Default with {TradeOrId = trade}).Build())
            .ToList();

        DbContext.Trades.Add(trade);
        DbContext.References.AddRange(references);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var referenceRequestModel =
            new AddReferenceRequestModel(trade.Id, "https://example.org", "some notes");

        var response = await Interactor.Execute(referenceRequestModel, TestContext.Current.CancellationToken);

        var conflict = Assert.IsType<Conflict>(response.Value);
        Assert.Equal(trade.Id, conflict.ResourceId);
        Assert.Equal("You can't add more than 5 references per trade.", conflict.Details);
    }

    [Fact]
    public async Task A_reference_gets_successfully_added()
    {
        var trade = TestData.Trade.Default.Build();

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var referenceRequestModel =
            new AddReferenceRequestModel(trade.Id, "https://example.org");

        var response = await Interactor.Execute(referenceRequestModel, TestContext.Current.CancellationToken);

        var result = Assert.IsType<Completed<Guid>>(response.Value);
        var referenceId = result.Data;
        var tradeWithAddedReference = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        Assert.NotNull(tradeWithAddedReference);
        var reference = Assert.Single(tradeWithAddedReference.References);
        Assert.Equal(referenceId, reference.Id);
    }
    
    [Fact]
    public async Task A_reference_with_more_than_2000_characters_is_not_allowed()
    {
        var trade = TestData.Trade.Default.Build();

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var referenceRequestModel =
            new AddReferenceRequestModel(trade.Id, $"https://example.org", new string('a', 2001));

        var response = await Interactor.Execute(referenceRequestModel, TestContext.Current.CancellationToken);

        var result = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(result.Errors);
        Assert.Equal("The length of 'Notes' must be 2000 characters or fewer. You entered 2001 characters.", error.ErrorMessage);
    }
}