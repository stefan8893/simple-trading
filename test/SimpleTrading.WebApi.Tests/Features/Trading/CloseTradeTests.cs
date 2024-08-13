using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Client;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading;

public class CloseTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-04T12:00").ToUtcKind();

    [Fact]
    public async Task A_request_without_an_access_token_is_not_authorized()
    {
        // arrange
        var client = Factory.CreateClient();
        var simpleTradingClient = new SimpleTradingClient(client);

        var tradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        var act = () => simpleTradingClient.CloseTradeAsync(tradeId, new CloseTradeDto
        {
            Closed = _utcNow,
            Result = ResultDto.Loss,
            Balance = -20d,
            ExitPrice = 1.05
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task The_trade_to_close_was_not_found()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        var act = () => simpleTradingClient.CloseTradeAsync(notExistingTradeId, new CloseTradeDto
        {
            Closed = _utcNow,
            Result = ResultDto.Loss,
            Balance = -20d,
            ExitPrice = 1.05
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        exception.Which.Result.FieldErrors.Should().BeEmpty();
        exception.Which.Result.CommonErrors
            .Should().Contain(x => x == "Trade nicht gefunden.")
            .And.HaveCount(1);
    }

    [Fact]
    public async Task The_balance_must_not_be_null()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        var act = () => simpleTradingClient.CloseTradeAsync(notExistingTradeId, new CloseTradeDto
        {
            Closed = _utcNow,
            Result = ResultDto.BreakEven,
            Balance = null,
            ExitPrice = 1.05
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.CommonErrors.Should().BeEmpty();

        exception.Which.Result.FieldErrors
            .Should().HaveCount(1)
            .And.Contain(x => x.Identifier == "Balance")
            .And.Contain(x => x.Messages.Count == 1)
            .And.Contain(x => x.Messages.Single() == "'Bilanz' darf kein Nullwert sein.");
    }

    [Fact]
    public async Task The_closed_date_must_not_be_null()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        var act = () => simpleTradingClient.CloseTradeAsync(notExistingTradeId, new CloseTradeDto
        {
            Closed = null,
            Result = ResultDto.BreakEven,
            Balance = 0d,
            ExitPrice = 1.05
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.CommonErrors.Should().BeEmpty();

        exception.Which.Result.FieldErrors
            .Should().HaveCount(1)
            .And.Contain(x => x.Identifier == "Closed")
            .And.Contain(x => x.Messages.Count == 1)
            .And.Contain(x => x.Messages.Single() == "'Abgeschlossen' darf kein Nullwert sein.");
    }

    [Fact]
    public async Task Unprocessable_entity_response_if_closed_date_is_before_opened_date()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => simpleTradingClient.CloseTradeAsync(trade.Id, new CloseTradeDto
        {
            Closed = _utcNow.AddDays(-1),
            Result = ResultDto.Loss,
            Balance = -50d,
            ExitPrice = 1.05
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        exception.Which.Result.FieldErrors.Should().BeEmpty();
        exception.Which.Result.CommonErrors
            .Should().HaveCount(1)
            .And.Contain(x => x == "Das Datum 'Abgeschlossen' muss nach dem Datum 'Eröffnet' sein.");
    }

    [Fact]
    public async Task A_trade_can_be_successfully_closed()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => simpleTradingClient.CloseTradeAsync(trade.Id, new CloseTradeDto
        {
            Closed = _utcNow,
            Result = ResultDto.Loss,
            Balance = -50d,
            ExitPrice = 1.05
        });

        // assert
        await act.Should().NotThrowAsync();
        var tradeAfterClosing = await DbContext.Trades
            .AsNoTracking()
            .FirstAsync(x => x.Id == trade.Id);

        tradeAfterClosing.Should().NotBeNull();
        tradeAfterClosing!.IsClosed.Should().BeTrue();
    }
    
    
    [Fact]
    public async Task A_trade_gets_closed_in_new_york_local_time_but_the_date_is_stored_in_utc()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var closedInNewYork = DateTimeOffset.Parse("2024-08-05T12:00:00-04:00");

        // act
        var act = () => simpleTradingClient.CloseTradeAsync(trade.Id, new CloseTradeDto
        {
            Closed = closedInNewYork,
            Result = ResultDto.Loss,
            Balance = -50d,
            ExitPrice = 1.05
        });

        // assert
        await act.Should().NotThrowAsync();
        var tradeAfterClosing = await DbContext.Trades
            .AsNoTracking()
            .FirstAsync(x => x.Id == trade.Id);

        tradeAfterClosing.Should().NotBeNull();
        var expectedClosedDate = DateTime.Parse("2024-08-05T16:00:00");
        tradeAfterClosing.Closed.Should().HaveValue()
            .And.Be(expectedClosedDate);
    }
}