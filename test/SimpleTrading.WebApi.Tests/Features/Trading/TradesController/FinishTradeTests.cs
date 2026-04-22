using Microsoft.AspNetCore.Http;
using SimpleTrading.Client;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.TradesController;

public class FinishTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-04T12:00").ToUtcKind();

    [Fact]
    public async Task A_request_without_an_access_token_is_not_authorized()
    {
        var client = await CreateClient(false);

        var tradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act()
        {
            return client.FinishTradeAsync(tradeId,
                new FinishTradeDto {Finished = new DateTimeOffset(_utcNow), ProfitLoss = -20d, ExitPrice = 1.05});
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status401Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task The_trade_to_finish_was_not_found()
    {
        var client = await CreateClient();

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act()
        {
            return client.FinishTradeAsync(notExistingTradeId,
                new FinishTradeDto {Finished = new DateTimeOffset(_utcNow), ProfitLoss = -20d, ExitPrice = 1.05});
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.Equal("Trade nicht gefunden.", exception.Result.Title);
        Assert.Equal($"Trade mit der ID '{notExistingTradeId}' nicht gefunden.", exception.Result.Detail);
        
    }

    [Fact]
    public async Task The_profitLoss_must_not_be_null()
    {
        var client = await CreateClient();

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act()
        {
            return client.FinishTradeAsync(notExistingTradeId,
                new FinishTradeDto {Finished = new DateTimeOffset(_utcNow), ProfitLoss = null, ExitPrice = 1.05});
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("profitLoss", error.Key);
        Assert.Equal("'Gewinn/Verlust' darf kein Nullwert sein.", Assert.Single(error.Value));
    }

    [Fact]
    public async Task The_finished_date_must_not_be_null()
    {
        var client = await CreateClient();

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act()
        {
            return client.FinishTradeAsync(notExistingTradeId,
                new FinishTradeDto {Finished = null, ProfitLoss = 0d, ExitPrice = 1.05});
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("finished", error.Key);
        Assert.Equal("'Abgeschlossen' darf kein Nullwert sein.", Assert.Single(error.Value));
    }

    [Fact]
    public async Task Returns_conflict_when_finished_date_is_before_opened_date()
    {
        var client = await CreateClient();

        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act()
        {
            return client.FinishTradeAsync(trade.Id,
                new FinishTradeDto
                    {Finished = new DateTimeOffset(_utcNow).AddDays(-1), ProfitLoss = -50d, ExitPrice = 1.05});
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status409Conflict, exception.StatusCode);
        Assert.Equal("'Abgeschlossen' muss nach 'Eröffnet' liegen.", exception.Result.Detail);
    }

    [Fact]
    public async Task A_trade_can_be_successfully_finished()
    {
        var client = await CreateClient();

        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await client.FinishTradeAsync(trade.Id, new FinishTradeDto
        {
            Finished = new DateTimeOffset(_utcNow),
            ProfitLoss = -50d,
            ExitPrice = 1.05
        }, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        var tradeAfterClosing = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        Assert.NotNull(tradeAfterClosing);
        Assert.True(tradeAfterClosing.IsFinished);
    }

    [Fact]
    public async Task The_result_gets_overriden()
    {
        var client = await CreateClient();

        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await client.FinishTradeAsync(trade.Id, new FinishTradeDto
        {
            Finished = new DateTimeOffset(_utcNow),
            ProfitLoss = -50d,
            ExitPrice = 1.05
        }, TestContext.Current.CancellationToken);

        var tradeAfterClosing = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        Assert.NotNull(tradeAfterClosing);
        Assert.True(tradeAfterClosing.IsFinished);
    }

    [Fact]
    public async Task A_trade_gets_finished_in_new_york_local_time_but_the_date_is_stored_in_utc()
    {
        var client = await CreateClient();

        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var finishedInNewYork = DateTimeOffset.Parse("2024-08-05T12:00:00-04:00");

        var result = await client.FinishTradeAsync(trade.Id, new FinishTradeDto
        {
            Finished = finishedInNewYork,
            ProfitLoss = -50d,
            ExitPrice = 1.05
        }, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        var tradeAfterClosing = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        Assert.NotNull(tradeAfterClosing);
        var expectedFinisheddDate = DateTime.Parse("2024-08-05T16:00:00");
        Assert.True(tradeAfterClosing.Finished.HasValue);
        Assert.Equal(expectedFinisheddDate, tradeAfterClosing.Finished.Value);
    }
}