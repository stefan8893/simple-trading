using Autofac;
using FluentValidation.Results;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.References.UpdateReference;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.References;

public class UpdateReferenceTests : DomainTests
{
    private IUpdateReference Interactor => ServiceLocator.Resolve<IUpdateReference>();

    [Fact]
    public async Task A_trades_reference_can_be_successfully_updated()
    {
        var trade = TestData.Trade.Default.Build();
        var reference = (TestData.Reference.Default with
        {
            TradeOrId = trade,
            Notes = "Some Notes"
        }).Build();

        DbContext.AddRange(trade, reference);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var updateReferenceRequestModel = new UpdateReferenceRequestModel
        {
            TradeId = trade.Id,
            ReferenceId = reference.Id,
            Notes = null
        };

        var response = await Interactor.Execute(updateReferenceRequestModel, TestContext.Current.CancellationToken);

        Assert.IsType<Completed>(response.Value);

        var updatedReference = await DbContextSingleOrDefault<Reference>(x => x.Id == reference.Id);

        Assert.NotNull(updatedReference);
        Assert.Null(updatedReference.Notes);
    }

    [Fact]
    public async Task You_cannot_update_references_of_a_non_existing_trade()
    {
        var notExistingTradeId = Guid.Parse("e5a40443-6a65-4bc1-9141-3ae859c0a665");
        var reference = (TestData.Reference.Default with
        {
            Notes = "Some Notes"
        }).Build();

        DbContext.AddRange(reference);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var updateReferenceRequestModel = new UpdateReferenceRequestModel
        {
            TradeId = notExistingTradeId,
            ReferenceId = reference.Id,
            Notes = null
        };

        var response = await Interactor.Execute(updateReferenceRequestModel, TestContext.Current.CancellationToken);

        var notFound = Assert.IsType<NotFound<Trade>>(response.Value);
        Assert.Equal(notExistingTradeId, notFound.ResourceId);
    }

    [Fact]
    public async Task You_cannot_update_references_of_a_non_existing_reference()
    {
        var trade = TestData.Trade.Default.Build();
        var notExistingReferenceId = Guid.Parse("7ddbfd72-4e97-499d-9fff-7f1615eae562");

        DbContext.AddRange(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var updateReferenceRequestModel = new UpdateReferenceRequestModel
        {
            TradeId = trade.Id,
            ReferenceId = notExistingReferenceId,
            Notes = "updated note"
        };

        var response = await Interactor.Execute(updateReferenceRequestModel, TestContext.Current.CancellationToken);

        var notFound = Assert.IsType<NotFound<Reference>>(response.Value);
        Assert.Equal(notExistingReferenceId, notFound.ResourceId);
    }

    [Fact]
    public async Task A_reference_type_out_of_enum_range_is_not_allowed()
    {
        var trade = TestData.Trade.Default.Build();
        var reference = (TestData.Reference.Default with
        {
            TradeOrId = trade,
            Type = ReferenceType.Other,
            Link = new Uri("https://example.org")
        }).Build();

        trade.References.Add(reference);

        var referenceRequestModel =
            new UpdateReferenceRequestModel
            {
                TradeId = trade.Id,
                ReferenceId = reference.Id,
                Type = (ReferenceType) 50
            };

        var response = await Interactor.Execute(referenceRequestModel, TestContext.Current.CancellationToken);

        var validationResult = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("'Reference Type' has a range of values which does not include '50'.", error.ErrorMessage);
        Assert.Equal("Type", error.PropertyName);
    }

    [Fact]
    public async Task A_reference_link_must_be_a_valid_uri()
    {
        var trade = TestData.Trade.Default.Build();
        var reference = (TestData.Reference.Default with
        {
            TradeOrId = trade,
            Type = ReferenceType.Other,
            Link = new Uri("https://example.org")
        }).Build();

        trade.References.Add(reference);

        var referenceRequestModel =
            new UpdateReferenceRequestModel
            {
                TradeId = trade.Id,
                ReferenceId = reference.Id,
                Link = "not-valid-uri"
            };

        var response = await Interactor.Execute(referenceRequestModel, TestContext.Current.CancellationToken);

        var validationResult = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("Invalid link.", error.ErrorMessage);
        Assert.Equal("Link", error.PropertyName);
    }

    [Fact]
    public async Task Reference_notes_must_not_contain_more_than_4000_chars()
    {
        var trade = TestData.Trade.Default.Build();
        var reference = (TestData.Reference.Default with
        {
            TradeOrId = trade,
            Type = ReferenceType.Other,
            Link = new Uri("https://example.org")
        }).Build();

        trade.References.Add(reference);

        var referenceRequestModel =
            new UpdateReferenceRequestModel
            {
                TradeId = trade.Id,
                ReferenceId = reference.Id,
                Notes = new string('a', 4001)
            };

        var response = await Interactor.Execute(referenceRequestModel, TestContext.Current.CancellationToken);

        var validationResult = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("The length of 'Notes' must be 4000 characters or fewer. You entered 4001 characters.",
            error.ErrorMessage);
        Assert.Equal("Notes", error.PropertyName);
    }
}