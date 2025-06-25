using AwesomeAssertions;
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
        response.Should().NotBeNull();
        response.Warnings.Should().BeEmpty();
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);

        updatedTrade.Should().NotBeNull();
        updatedTrade!.Size.Should().Be(50_000);
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
        response.Should().NotBeNull();
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);

        updatedTrade.Should().NotBeNull();
        updatedTrade!.Result.Should().NotBeNull();
        updatedTrade.Result!.Name.Should().Be(Result.Loss);
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
        var act = () => client.UpdateTradeAsync(trade.Id, new UpdateTradeDto
        {
            ManuallyEnteredResult = new ResultDtoNullableUpdateValue {Value = ResultDto.Loss}
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<FieldErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.Errors.Should().HaveCount(1)
            .And.Contain(x => x.Identifier == "ManuallyEnteredResult" &&
                              x.Messages.Single() ==
                              "'Ergebnis' kann nur aktualisiert werden, wenn der Trade bereits abgeschlossen ist.");
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
        response.Should().NotBeNull();
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);

        updatedTrade.Should().NotBeNull();
        updatedTrade!.Result.Should().BeNull();
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
        response.Should().NotBeNull();
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);

        updatedTrade.Should().NotBeNull();
        updatedTrade!.Result.Should().NotBeNull();
        updatedTrade.Result!.Name.Should().Be(Result.BreakEven);
        updatedTrade.Result!.Source.Should().Be(ResultSource.CalculatedByBalance);
    }

    [Fact]
    public async Task An_non_existing_trade_cannot_be_updated()
    {
        // arrange
        var client = await CreateClient();

        var notExistingTradeId = Guid.Parse("74af4aee-9582-49ab-956a-1fd7d6f8609d");

        // act
        var act = () => client.UpdateTradeAsync(notExistingTradeId, new UpdateTradeDto());

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
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
        var act = () => client.UpdateTradeAsync(trade.Id, new UpdateTradeDto
        {
            Closed = DateTimeOffset.Parse("2024-08-14T17:00:00")
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<FieldErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.Errors.Should().HaveCount(1)
            .And.Contain(x => x.Identifier == "Closed" &&
                              x.Messages.Single() ==
                              "'Abgeschlossen' kann nur aktualisiert werden, wenn der Trade bereits abgeschlossen ist.");
    }
}