﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.References.GetReferences;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.References;

public class GetReferencesTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private IGetReferences CreateInteractor()
    {
        return ServiceLocator.GetRequiredService<IGetReferences>();
    }

    [Fact]
    public async Task A_non_existing_references_cant_be_returned()
    {
        var notExistingTradeId = Guid.Parse("a622d632-a7ef-42fe-adfa-fcb917e65926");

        var response = await CreateInteractor()
            .Execute(new GetReferencesRequestModel(notExistingTradeId));

        response.Value.Should().BeOfType<NotFound<Trade>>()
            .Which.ResourceId.Should().Be(notExistingTradeId);
    }

    [Fact]
    public async Task A_trades_references_will_be_returned()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var reference1 = (TestData.Reference.Default with {TradeOrId = trade}).Build();
        var reference2 = (TestData.Reference.Default with {TradeOrId = trade}).Build();

        DbContext.AddRange(trade, reference1, reference2);
        await DbContext.SaveChangesAsync();

        // act
        var response = await CreateInteractor()
            .Execute(new GetReferencesRequestModel(trade.Id));

        // assert
        var responseModel = response.Value.Should().BeAssignableTo<IReadOnlyList<ReferenceModel>>();
        responseModel.Which.Should().HaveCount(2)
            .And.Contain(x => x.Id == reference1.Id)
            .And.Contain(x => x.Id == reference2.Id);
    }
}