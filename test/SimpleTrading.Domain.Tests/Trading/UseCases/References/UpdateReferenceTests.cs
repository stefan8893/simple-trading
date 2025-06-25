using Autofac;
using AwesomeAssertions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.References.UpdateReference;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.References;

public class UpdateReferenceTests : DomainTests
{
    private IUpdateReference Interactor => ServiceLocator.Resolve<IUpdateReference>();

    [Fact]
    public async Task A_trades_reference_can_be_successfully_updated()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var reference = (TestData.Reference.Default with
        {
            TradeOrId = trade,
            Notes = "Some Notes"
        }).Build();

        DbContext.AddRange(trade, reference);
        await DbContext.SaveChangesAsync();

        var updateReferenceRequestModel = new UpdateReferenceRequestModel
        {
            TradeId = trade.Id,
            ReferenceId = reference.Id,
            Notes = null
        };

        // act
        var response = await Interactor.Execute(updateReferenceRequestModel);

        // assert
        response.Value.Should().BeOfType<Completed>();

        var updatedReference = await DbContextSingleOrDefault<Reference>(x => x.Id == reference.Id);

        updatedReference.Should().NotBeNull();
        updatedReference.Notes.Should().BeNull();
    }

    [Fact]
    public async Task You_cannot_update_references_of_a_non_existing_trade()
    {
        // arrange
        var notExistingTradeId = Guid.Parse("e5a40443-6a65-4bc1-9141-3ae859c0a665");
        var reference = (TestData.Reference.Default with
        {
            Notes = "Some Notes"
        }).Build();

        DbContext.AddRange(reference);
        await DbContext.SaveChangesAsync();

        var updateReferenceRequestModel = new UpdateReferenceRequestModel
        {
            TradeId = notExistingTradeId,
            ReferenceId = reference.Id,
            Notes = null
        };

        // act
        var response = await Interactor.Execute(updateReferenceRequestModel);

        // assert
        var notFound = response.Value.Should().BeOfType<NotFound<Trade>>();
        notFound.Which.ResourceId.Should().Be(notExistingTradeId);
    }

    [Fact]
    public async Task You_cannot_update_references_of_a_non_existing_reference()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var notExistingReferenceId = Guid.Parse("7ddbfd72-4e97-499d-9fff-7f1615eae562");

        DbContext.AddRange(trade);
        await DbContext.SaveChangesAsync();

        var updateReferenceRequestModel = new UpdateReferenceRequestModel
        {
            TradeId = trade.Id,
            ReferenceId = notExistingReferenceId,
            Notes = "updated note"
        };

        // act
        var response = await Interactor.Execute(updateReferenceRequestModel);

        // assert
        var notFound = response.Value.Should().BeOfType<NotFound<Reference>>();
        notFound.Which.ResourceId.Should().Be(notExistingReferenceId);
    }

    [Fact]
    public async Task A_reference_type_out_of_enum_range_is_not_allowed()
    {
        // arrange
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

        // act
        var response = await Interactor.Execute(referenceRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Reference type' has a range of values which does not include '50'.")
            .And.Contain(x => x.PropertyName == "Type");
    }

    [Fact]
    public async Task A_reference_link_must_be_a_valid_uri()
    {
        // arrange
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

        // act
        var response = await Interactor.Execute(referenceRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "Invalid link.")
            .And.Contain(x => x.PropertyName == "Link");
    }

    [Fact]
    public async Task Reference_notes_must_not_contain_more_than_4000_chars()
    {
        // arrange
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

        // act
        var response = await Interactor.Execute(referenceRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x =>
                x.ErrorMessage ==
                "The length of 'Notes' must be 4000 characters or fewer. You entered 4001 characters.")
            .And.Contain(x => x.PropertyName == "Notes");
    }
}