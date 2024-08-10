using System.Globalization;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.CloseTrade;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class CloseTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private readonly UtcNow _utcNow = () => DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();

    protected override void OverrideServices(WebHostBuilderContext ctx, IServiceCollection services)
    {
        base.OverrideServices(ctx, services);
        services.AddSingleton<UtcNow>(_ => _utcNow);
    }

    private ICloseTrade CreateInteractor()
    {
        return ServiceLocator.GetRequiredService<ICloseTrade>();
    }

    [Fact]
    public async Task Invalid_Result_input_with_a_different_ui_culture_returns_a_localized_error_message()
    {
        // arrange
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-AT");

        var requestModel = new CloseTradeRequestModel(Guid.NewGuid(),
            (Result) 50,
            0m,
            1.05m,
            DateTime.Parse("2024-08-03T16:00:00+00:00"));

        // act
        var response = await CreateInteractor().Execute(requestModel);

        // assert
        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors
            .Should().Contain(x => x.ErrorMessage == "'Ergebnis' hat einen Wertebereich, der '50' nicht enthält.")
            .And.Contain(x => x.PropertyName == "Result")
            .And.HaveCount(1);
    }

    [Fact]
    public async Task A_not_existing_trade_cannot_be_closed()
    {
        var tradeId = Guid.Parse("2b58e712-e7d4-4df2-8a62-c9baac5ee889");
        var requestModel =
            new CloseTradeRequestModel(tradeId, Result.Win, 500, 1.05m, DateTime.Parse("2024-08-03T16:00:00Z"));

        var response = await CreateInteractor().Execute(requestModel);

        var notFound = response.Value.Should().BeOfType<NotFound>();
        notFound.Which.ResourceType.Should().Be("Trade");
        notFound.Which.ResourceId.Should().Be(tradeId);
    }

    [Fact]
    public async Task A_trades_exit_price_must_be_greater_than_zero()
    {
        // arrange
        var trade = (TestData.Trade.Default with {Opened = _utcNow()}).Build();
        DbContext.Add(trade);
        await DbContext.SaveChangesAsync();

        var requestModel =
            new CloseTradeRequestModel(trade.Id, Result.Win, 500, 0m, _utcNow().AddHours(1));

        // act
        var response = await CreateInteractor().Execute(requestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "ExitPrice");
    }
    
    [Fact]
    public async Task A_trade_can_be_closed_successfully()
    {
        // arrange
        var trade = (TestData.Trade.Default with {Opened = _utcNow()}).Build();
        DbContext.Add(trade);
        await DbContext.SaveChangesAsync();

        var requestModel =
            new CloseTradeRequestModel(trade.Id, Result.Win, 500, 1.05m, _utcNow().AddHours(1));

        // act
        var response = await CreateInteractor().Execute(requestModel);

        // assert
        response.Value.Should().BeOfType<Completed>();

        // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataQuery
        var closedTrade = await DbContext.Trades.AsNoTracking()
            .FirstAsync(x => x.Id == trade.Id);

        closedTrade.Balance.Should().Be(requestModel.Balance);
        closedTrade.Result.Should().Be(requestModel.Result);
        closedTrade.Closed.Should().NotBeNull();
        // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
        closedTrade.PositionPrices.ExitPrice.Should().NotBeNull().And.Be(requestModel.ExitPrice);
    }
}