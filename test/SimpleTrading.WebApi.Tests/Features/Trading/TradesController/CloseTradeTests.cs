using Microsoft.AspNetCore.Http;
using SimpleTrading.Client;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.TradesController;

public class CloseTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-04T12:00").ToUtcKind();

    [Fact]
    public async Task A_request_without_an_access_token_is_not_authorized()
    {
        // arrange
        var client = await CreateClient(false);

        var tradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        Task<TradeResultDto> Act()
        {
            return client.CloseTradeAsync(tradeId,
                new CloseTradeDto {Closed = new DateTimeOffset(_utcNow), ProfitLoss = -20d, ExitPrice = 1.05});
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status401Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task The_trade_to_close_was_not_found()
    {
        // arrange
        var client = await CreateClient();

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        Task<TradeResultDto> Act()
        {
            return client.CloseTradeAsync(notExistingTradeId,
                new CloseTradeDto {Closed = new DateTimeOffset(_utcNow), ProfitLoss = -20d, ExitPrice = 1.05});
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.Equal("Trade nicht gefunden.", exception.Result.Title);
        Assert.Equal($"Trade mit der ID '{notExistingTradeId}' nicht gefunden.", exception.Result.Detail);
        
    }

    [Fact]
    public async Task The_profitLoss_must_not_be_null()
    {
        // arrange
        var client = await CreateClient();

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act()
        {
            return client.CloseTradeAsync(notExistingTradeId,
                new CloseTradeDto {Closed = new DateTimeOffset(_utcNow), ProfitLoss = null, ExitPrice = 1.05});
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("profitLoss", error.Key);
        Assert.Equal("'Gewinn/Verlust' darf kein Nullwert sein.", Assert.Single(error.Value));
    }

    [Fact]
    public async Task The_closed_date_must_not_be_null()
    {
        // arrange
        var client = await CreateClient();

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act()
        {
            return client.CloseTradeAsync(notExistingTradeId,
                new CloseTradeDto {Closed = null, ProfitLoss = 0d, ExitPrice = 1.05});
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("closed", error.Key);
        Assert.Equal("'Abgeschlossen' darf kein Nullwert sein.", Assert.Single(error.Value));
    }

    [Fact]
    public async Task Returns_conflict_when_closed_date_is_before_opened_date()
    {
        // arrange
        var client = await CreateClient();

        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act()
        {
            return client.CloseTradeAsync(trade.Id,
                new CloseTradeDto
                    {Closed = new DateTimeOffset(_utcNow).AddDays(-1), ProfitLoss = -50d, ExitPrice = 1.05});
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status409Conflict, exception.StatusCode);
        Assert.Equal("'Abgeschlossen' muss nach 'Eröffnet' liegen.", exception.Result.Detail);
    }

    [Fact]
    public async Task A_trade_can_be_successfully_closed()
    {
        // arrange
        var client = await CreateClient();

        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var result = await client.CloseTradeAsync(trade.Id, new CloseTradeDto
        {
            Closed = new DateTimeOffset(_utcNow),
            ProfitLoss = -50d,
            ExitPrice = 1.05
        }, TestContext.Current.CancellationToken);

        // assert
        Assert.NotNull(result);
        var tradeAfterClosing = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        Assert.NotNull(tradeAfterClosing);
        Assert.True(tradeAfterClosing.IsClosed);
    }

    [Fact]
    public async Task The_result_gets_overriden()
    {
        // arrange
        var client = await CreateClient();

        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        await client.CloseTradeAsync(trade.Id, new CloseTradeDto
        {
            Closed = new DateTimeOffset(_utcNow),
            ProfitLoss = -50d,
            ExitPrice = 1.05
        }, TestContext.Current.CancellationToken);

        // assert
        var tradeAfterClosing = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        Assert.NotNull(tradeAfterClosing);
        Assert.True(tradeAfterClosing.IsClosed);
    }

    [Fact]
    public async Task A_trade_gets_closed_in_new_york_local_time_but_the_date_is_stored_in_utc()
    {
        // arrange
        var client = await CreateClient();

        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var closedInNewYork = DateTimeOffset.Parse("2024-08-05T12:00:00-04:00");

        // act
        var result = await client.CloseTradeAsync(trade.Id, new CloseTradeDto
        {
            Closed = closedInNewYork,
            ProfitLoss = -50d,
            ExitPrice = 1.05
        }, TestContext.Current.CancellationToken);

        // assert
        Assert.NotNull(result);
        var tradeAfterClosing = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        Assert.NotNull(tradeAfterClosing);
        var expectedClosedDate = DateTime.Parse("2024-08-05T16:00:00");
        Assert.True(tradeAfterClosing.Closed.HasValue);
        Assert.Equal(expectedClosedDate, tradeAfterClosing.Closed.Value);
    }
}