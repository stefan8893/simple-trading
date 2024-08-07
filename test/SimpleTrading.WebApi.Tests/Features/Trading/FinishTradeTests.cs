using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Client;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading;

public class FinishTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-04T12:00").ToUtcKind();

    [Fact]
    public async Task A_request_without_an_access_token_is_not_authorized()
    {
        // arrange
        var client = Factory.CreateClient();
        var simpleTradingClient = new SimpleTradingClient(client);

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        var act = () => simpleTradingClient.FinishTradeAsync(notExistingTradeId, new FinishTradeDto
        {
            FinishedAt = _utcNow,
            Result = ResultDto.Loss,
            Balance = -20d
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task The_trade_to_finish_was_not_found()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        var act = () => simpleTradingClient.FinishTradeAsync(notExistingTradeId, new FinishTradeDto
        {
            FinishedAt = _utcNow,
            Result = ResultDto.Loss,
            Balance = -20d
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
    public async Task The_result_must_not_be_null()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        var act = () => simpleTradingClient.FinishTradeAsync(notExistingTradeId, new FinishTradeDto
        {
            FinishedAt = _utcNow,
            Result = null,
            Balance = -5d
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.CommonErrors.Should().BeEmpty();

        exception.Which.Result.FieldErrors
            .Should().HaveCount(1)
            .And.Contain(x => x.Identifier == "Result")
            .And.Contain(x => x.Messages.Count == 1)
            .And.Contain(x => x.Messages.First() == "'Ergebnis' darf kein Nullwert sein.");
    }

    [Fact]
    public async Task The_balance_must_not_be_null()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        var act = () => simpleTradingClient.FinishTradeAsync(notExistingTradeId, new FinishTradeDto
        {
            FinishedAt = _utcNow,
            Result = ResultDto.BreakEven,
            Balance = null
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
    public async Task The_finished_date_must_not_be_null()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        var act = () => simpleTradingClient.FinishTradeAsync(notExistingTradeId, new FinishTradeDto
        {
            FinishedAt = null,
            Result = ResultDto.BreakEven,
            Balance = 0d
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.CommonErrors.Should().BeEmpty();

        exception.Which.Result.FieldErrors
            .Should().HaveCount(1)
            .And.Contain(x => x.Identifier == "FinishedAt")
            .And.Contain(x => x.Messages.Count == 1)
            .And.Contain(x => x.Messages.Single() == "'Beendet' darf kein Nullwert sein.");
    }

    [Fact]
    public async Task Bad_request_response_if_balance_is_null()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var notExistingTradeId = Guid.Parse("81e0c3a0-ce71-405d-a6db-a53d4b201c8b");

        // act
        var act = () => simpleTradingClient.FinishTradeAsync(notExistingTradeId, new FinishTradeDto
        {
            FinishedAt = _utcNow,
            Result = ResultDto.Loss,
            Balance = null
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.CommonErrors.Should().BeEmpty();

        exception.Which.Result.FieldErrors
            .Should().HaveCount(1)
            .And.Contain(x => x.Identifier == "Balance")
            .And.Contain(x => x.Messages.Count == 1)
            .And.Contain(x => x.Messages.First() == "'Bilanz' darf kein Nullwert sein.");
    }

    [Fact]
    public async Task Unprocessable_entity_response_if_finished_date_is_before_opened_date()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var trade = (TestData.Trade.Default with {OpenedAt = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => simpleTradingClient.FinishTradeAsync(trade.Id, new FinishTradeDto
        {
            FinishedAt = _utcNow.AddDays(-1),
            Result = ResultDto.Loss,
            Balance = -50d
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        exception.Which.Result.FieldErrors.Should().BeEmpty();
        exception.Which.Result.CommonErrors
            .Should().HaveCount(1)
            .And.Contain(x => x == "Das Datum 'Beendet' muss nach dem Datum 'Eröffnet' liegen.");
    }

    [Fact]
    public async Task A_trade_can_be_successfully_finished()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var trade = (TestData.Trade.Default with {OpenedAt = _utcNow}).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => simpleTradingClient.FinishTradeAsync(trade.Id, new FinishTradeDto
        {
            FinishedAt = _utcNow,
            Result = ResultDto.Loss,
            Balance = -50d
        });

        // assert
        await act.Should().NotThrowAsync();

        var tradeAfterFinishing = await DbContext.Trades
            .AsNoTracking()
            .FirstAsync(x => x.Id == trade.Id);

        tradeAfterFinishing.Should().NotBeNull();
        tradeAfterFinishing!.IsFinished.Should().BeTrue();
    }
}