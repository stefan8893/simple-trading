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
                new CloseTradeDto {Closed = new DateTimeOffset(_utcNow), Balance = -20d, ExitPrice = 1.05});
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException>(Act);
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
                new CloseTradeDto {Closed = new DateTimeOffset(_utcNow), Balance = -20d, ExitPrice = 1.05});
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        var singleError = Assert.Single(exception.Result.Messages);
        Assert.Equal("Trade nicht gefunden.", singleError);
    }

    [Fact]
    public async Task The_balance_must_not_be_null()
    {
        // arrange
        var client = await CreateClient();

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act()
        {
            return client.CloseTradeAsync(notExistingTradeId,
                new CloseTradeDto {Closed = new DateTimeOffset(_utcNow), Balance = null, ExitPrice = 1.05});
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<FieldErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("balance", error.Identifier);
        Assert.Equal("'Bilanz' darf kein Nullwert sein.", Assert.Single(error.Messages));
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
                new CloseTradeDto {Closed = null, Balance = 0d, ExitPrice = 1.05});
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<FieldErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("closed", error.Identifier);
        Assert.Equal("'Abgeschlossen' darf kein Nullwert sein.", Assert.Single(error.Messages));
    }

    [Fact]
    public async Task Unprocessable_entity_response_if_closed_date_is_before_opened_date()
    {
        // arrange
        var client = await CreateClient();

        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act()
        {
            return client.CloseTradeAsync(trade.Id,
                new CloseTradeDto {Closed = new DateTimeOffset(_utcNow).AddDays(-1), Balance = -50d, ExitPrice = 1.05});
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, exception.StatusCode);
        var singleError = Assert.Single(exception.Result.Messages);
        Assert.Equal("'Abgeschlossen' muss nach 'Eröffnet' liegen.", singleError);
    }

    [Fact]
    public async Task A_trade_can_be_successfully_closed()
    {
        // arrange
        var client = await CreateClient();

        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var result = await client.CloseTradeAsync(trade.Id, new CloseTradeDto
        {
            Closed = new DateTimeOffset(_utcNow),
            Balance = -50d,
            ExitPrice = 1.05
        });

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
        await DbContext.SaveChangesAsync();

        // act
        await client.CloseTradeAsync(trade.Id, new CloseTradeDto
        {
            Closed = new DateTimeOffset(_utcNow),
            Balance = -50d,
            ExitPrice = 1.05
        });

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
        await DbContext.SaveChangesAsync();

        var closedInNewYork = DateTimeOffset.Parse("2024-08-05T12:00:00-04:00");

        // act
        var result = await client.CloseTradeAsync(trade.Id, new CloseTradeDto
        {
            Closed = closedInNewYork,
            Balance = -50d,
            ExitPrice = 1.05
        });

        // assert
        Assert.NotNull(result);
        var tradeAfterClosing = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        Assert.NotNull(tradeAfterClosing);
        var expectedClosedDate = DateTime.Parse("2024-08-05T16:00:00");
        Assert.True(tradeAfterClosing.Closed.HasValue);
        Assert.Equal(expectedClosedDate, tradeAfterClosing.Closed.Value);
    }
}