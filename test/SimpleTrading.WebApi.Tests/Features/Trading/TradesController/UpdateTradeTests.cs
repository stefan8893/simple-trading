using Microsoft.AspNetCore.Http;
using SimpleTrading.Client;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.TradesController;

public class UpdateTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task A_trades_size_can_be_successfully_updated()
    {
        // arrange
        var client = await CreateClient();

        var trade = (TestData.Trade.Default with
        {
            Size = 5000
        }).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var response = await client.UpdateTradeAsync(trade.Id, new UpdateTradeDto
        {
            Size = 50_000
        });

        // assert
        Assert.NotNull(response);
        Assert.Empty(response.Warnings);
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);

        Assert.NotNull(updatedTrade);
        Assert.Equal(50_000, updatedTrade.Size);
    }

    [Fact]
    public async Task A_trades_result_can_be_successfully_updated_since_the_trade_is_closed()
    {
        // arrange
        var client = await CreateClient();
        var now = DateTime.Parse("2024-09-22T10:00:00").ToUtcKind();

        var trade = (TestData.Trade.Default with
        {
            Opened = now,
            Closed = now,
            Balance = 0m
        }).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var response = await client.UpdateTradeAsync(trade.Id, new UpdateTradeDto
        {
            ManuallyEnteredResult = new ResultDtoNullableUpdateValue {Value = ResultDto.Loss}
        });

        // assert
        Assert.NotNull(response);
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);

        Assert.NotNull(updatedTrade);
        Assert.NotNull(updatedTrade.Result);
        Assert.Equal(Result.Loss, updatedTrade.Result.Name);
    }

    [Fact]
    public async Task
        A_trades_result_cannot_be_successfully_updated_since_balance_and_closed_date_are_missing_and_the_trade_is_not_closed()
    {
        // arrange
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<WarningsDto> Act()
        {
            return client.UpdateTradeAsync(trade.Id,
                new UpdateTradeDto {ManuallyEnteredResult = new ResultDtoNullableUpdateValue {Value = ResultDto.Loss}});
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<FieldErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var singleError = Assert.Single(exception.Result.Errors);
        Assert.Equal("manuallyEnteredResult", singleError.Identifier);
        var singleMessage = Assert.Single(singleError.Messages);
        Assert.Equal("'Ergebnis' kann nur aktualisiert werden, wenn der Trade bereits abgeschlossen ist.",
            singleMessage);
    }

    [Fact]
    public async Task A_trades_result_can_be_successfully_updated_to_null_since_the_trade_is_closed()
    {
        // arrange
        var client = await CreateClient();
        var now = DateTime.Parse("2024-09-22T10:00:00").ToUtcKind();

        var trade = (TestData.Trade.Default with
        {
            Opened = now,
            Closed = now,
            Balance = 0m
        }).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var response = await client.UpdateTradeAsync(trade.Id, new UpdateTradeDto
        {
            ManuallyEnteredResult = new ResultDtoNullableUpdateValue {Value = null}
        });

        // assert
        Assert.NotNull(response);
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);

        Assert.NotNull(updatedTrade);
        Assert.Null(updatedTrade.Result);
    }

    [Fact]
    public async Task A_trades_result_will_not_be_updated_if_manually_entered_result_is_specified()
    {
        // arrange
        var client = await CreateClient();
        var now = DateTime.Parse("2024-09-22T10:00:00").ToUtcKind();

        var trade = (TestData.Trade.Default with
        {
            Opened = now,
            Closed = now,
            Balance = 0m
        }).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var response = await client.UpdateTradeAsync(trade.Id, new UpdateTradeDto
        {
            ManuallyEnteredResult = null
        });

        // assert
        Assert.NotNull(response);
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);

        Assert.NotNull(updatedTrade);
        Assert.NotNull(updatedTrade.Result);
        Assert.Equal(Result.BreakEven, updatedTrade.Result.Name);
        Assert.Equal(ResultSource.CalculatedByBalance, updatedTrade.Result.Source);
    }

    [Fact]
    public async Task An_non_existing_trade_cannot_be_updated()
    {
        // arrange
        var client = await CreateClient();
        var notExistingTradeId = Guid.Parse("74af4aee-9582-49ab-956a-1fd7d6f8609d");

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<WarningsDto> Act()
        {
            return client.UpdateTradeAsync(notExistingTradeId, new UpdateTradeDto());
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task The_closed_date_of_a_non_closed_trade_cannot_be_updated()
    {
        // arrange
        var client = await CreateClient();

        var trade = TestData.Trade.Default.Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<WarningsDto> Act()
        {
            return client.UpdateTradeAsync(trade.Id,
                new UpdateTradeDto {Closed = DateTimeOffset.Parse("2024-08-14T17:00:00")});
        }

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<FieldErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var singleError = Assert.Single(exception.Result.Errors);
        Assert.Equal("closed", singleError.Identifier);
        var singleMessage = Assert.Single(singleError.Messages);
        Assert.Equal("'Abgeschlossen' kann nur aktualisiert werden, wenn der Trade bereits abgeschlossen ist.",
            singleMessage);
    }
}