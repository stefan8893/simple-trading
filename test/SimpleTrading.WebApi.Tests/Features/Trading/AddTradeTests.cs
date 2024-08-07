using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Client;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading;

public class AddTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-04T12:00").ToUtcKind();

    [Fact]
    public async Task A_request_without_an_access_token_is_not_authorized()
    {
        // arrange
        var client = Factory.CreateClient();
        var simpleTradingClient = new SimpleTradingClient(client);

        // act
        var act = () => simpleTradingClient.AddTradeAsync();

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task A_trade_can_be_successfully_added()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        // act
        var tradeId = await simpleTradingClient.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            OpenedAt = _utcNow,
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        var newlyAddedTrade = DbContext
            .Trades
            .AsNoTracking()
            .FirstAsync(x => x.Id == tradeId);

        newlyAddedTrade.Should().NotBeNull();
    }

    [Fact]
    public async Task TradeSize_must_not_be_null()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => simpleTradingClient.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            OpenedAt = _utcNow,
            Size = null,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.FieldErrors
            .Should().HaveCount(1)
            .And.Contain(x => x.Identifier == "Size")
            .And.Contain(x => x.Messages.Count == 1)
            .And.Contain(x => x.Messages.Single() == "'Handelsvolumen' darf kein Nullwert sein.");
    }

    [Fact]
    public async Task A_trade_cant_be_added_if_the_asset_is_missing()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var notExistingAssetId = Guid.Parse("a622d632-a7ef-42fe-adfa-fcb917e65926");
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(profile, currency);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => simpleTradingClient.AddTradeAsync(new AddTradeDto
        {
            AssetId = notExistingAssetId,
            ProfileId = profile.Id,
            OpenedAt = _utcNow,
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        exception.Which.Result.CommonErrors
            .Should().HaveCount(1)
            .And.Contain(x => x == "Asset nicht gefunden.");
    }

    [Fact]
    public async Task A_finished_trade_cant_be_added_the_balance_is_missing()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => simpleTradingClient.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            OpenedAt = _utcNow,
            FinishedAt = _utcNow,
            Result = ResultDto.Mediocre,
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        exception.Which.Result.CommonErrors
            .Should().HaveCount(1)
            .And.Contain(x =>
                x == "Um einen beendeten Trade hinzuzufügen, müssen Sie 'Beendet', 'Bilanz' und 'Ergebnis' angeben.");
    }
}