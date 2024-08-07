using System.Globalization;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.FinishTrade;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class FinishTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private readonly UtcNow _utcNow = () => DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();

    protected override void OverrideServices(WebHostBuilderContext ctx, IServiceCollection services)
    {
        base.OverrideServices(ctx, services);
        services.AddSingleton<UtcNow>(_ => _utcNow);
    }

    private IFinishTrade CreateInteractor()
    {
        return ServiceLocator.GetRequiredService<IFinishTrade>();
    }

    [Fact]
    public async Task Invalid_input_with_a_different_ui_culture_returns_a_localized_error_message()
    {
        // arrange
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-AT");

        var requestModel = new FinishTradeRequestModel(Guid.NewGuid(),
            (Result) 50,
            0m,
            DateTime.Parse("2024-08-03T16:00:00+00:00"));

        // act
        var response = await CreateInteractor().Execute(requestModel);

        // assert
        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors.Should()
            .Contain(x => x.ErrorMessage == "'Ergebnis' hat einen Wertebereich, der '50' nicht enthält.")
            .And.Contain(x => x.PropertyName == "Result")
            .And.HaveCount(1);
    }

    [Fact]
    public async Task A_not_existing_trade_cannot_be_finished()
    {
        var tradeId = Guid.Parse("2b58e712-e7d4-4df2-8a62-c9baac5ee889");
        var requestModel =
            new FinishTradeRequestModel(tradeId, Result.Win, 500, DateTime.Parse("2024-08-03T16:00:00Z"));

        var response = await CreateInteractor().Execute(requestModel);

        var notFound = response.Value.Should().BeOfType<NotFound>();
        notFound.Which.ResourceType.Should().Be("Trade");
        notFound.Which.ResourceId.Should().Be(tradeId);
    }

    [Fact]
    public async Task A_trade_can_be_finished_successfully()
    {
        // arrange
        var trade = (TestData.Trade.Default with {OpenedAt = _utcNow()}).Build();
        DbContext.Add(trade);
        await DbContext.SaveChangesAsync();

        var requestModel =
            new FinishTradeRequestModel(trade.Id, Result.Win, 500, _utcNow().AddHours(1));

        // act
        var response = await CreateInteractor().Execute(requestModel);

        // assert
        response.Value.Should().BeOfType<Completed>();
    }
}