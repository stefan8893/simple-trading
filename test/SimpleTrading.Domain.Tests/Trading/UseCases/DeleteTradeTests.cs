﻿using Autofac;
using AwesomeAssertions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.DeleteTrade;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class DeleteTradeTests : DomainTests
{
    private IDeleteTrade Interactor => ServiceLocator.Resolve<IDeleteTrade>();

    [Fact]
    public async Task A_not_existing_trade_cannot_be_deleted()
    {
        var notExistingTradeId = Guid.Parse("a47e07af-e0ae-49d0-8e1f-d0748f989c80");

        var response = await Interactor.Execute(new DeleteTradeRequestModel(notExistingTradeId));

        response.Value.Should().BeOfType<NotFound<Trade>>()
            .Which.ResourceId.Should().Be(notExistingTradeId);
    }

    [Fact]
    public async Task A_trade_can_be_successfully_deleted()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new DeleteTradeRequestModel(trade.Id));

        // assert
        response.Value.Should().BeOfType<Completed>();
        var storedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        storedTrade.Should().BeNull();
    }
}