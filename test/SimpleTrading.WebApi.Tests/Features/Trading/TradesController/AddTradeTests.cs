using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SimpleTrading.Client;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.TradesController;

public class AddTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-04T12:00").ToUtcKind();

    [Fact]
    public async Task A_request_without_an_access_token_is_not_authorized()
    {
        // arrange
        var client = await CreateClient(false);

        // act
        var act = () => client.AddTradeAsync();

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task A_trade_can_be_successfully_added()
    {
        // arrange
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        // act
        var response = await client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = _utcNow,
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        response.Should().NotBeNull();
        response.Warnings.Should().BeEmpty();
        response.Should().NotBeNull();
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);

        newlyAddedTrade.Should().NotBeNull();
    }

    [Fact]
    public async Task A_closed_trade_with_an_overriden_null_result_will_be_added()
    {
        // arrange
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        // act
        var response = await client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = _utcNow,
            Closed = _utcNow,
            Balance = 0,
            ManuallyEnteredResult = new ResultDtoNullableUpdateValue {Value = null},
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        response.Should().NotBeNull();
        response.Warnings.Should().BeEmpty();
        response.Should().NotBeNull();
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);

        newlyAddedTrade.Should().NotBeNull();
        newlyAddedTrade!.Result.Should().BeNull();
    }

    [Fact]
    public async Task A_closed_trade_with_an_overriden_result_will_be_added()
    {
        // arrange
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        // act
        var response = await client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = _utcNow,
            Closed = _utcNow,
            Balance = 0,
            ManuallyEnteredResult = new ResultDtoNullableUpdateValue {Value = ResultDto.Loss},
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        response.Should().NotBeNull();
        response.Should().NotBeNull();
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);

        newlyAddedTrade.Should().NotBeNull();
        newlyAddedTrade!.Result.Should().NotBeNull();
        newlyAddedTrade.Result!.Name.Should().Be(Result.Loss);
    }

    [Fact]
    public async Task TradeSize_must_not_be_null()
    {
        // arrange
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = _utcNow,
            Size = null,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<FieldErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.Errors
            .Should().HaveCount(1)
            .And.Contain(x => x.Identifier == "Size")
            .And.Contain(x => x.Messages.Single() == "'Handelsvolumen' darf kein Nullwert sein.");
    }

    [Fact]
    public async Task A_trade_cant_be_added_if_the_asset_is_missing()
    {
        // arrange
        var client = await CreateClient();

        var notExistingAssetId = Guid.Parse("a622d632-a7ef-42fe-adfa-fcb917e65926");
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(profile, currency);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => client.AddTradeAsync(new AddTradeDto
        {
            AssetId = notExistingAssetId,
            ProfileId = profile.Id,
            Opened = _utcNow,
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        exception.Which.Result.Messages
            .Should().HaveCount(1)
            .And.Contain(x => x == "Asset nicht gefunden.");
    }

    [Fact]
    public async Task A_closed_trade_cant_be_added_if_the_balance_is_missing()
    {
        // arrange
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = _utcNow,
            Closed = _utcNow,
            ManuallyEnteredResult = null,
            Size = 5000,
            Balance = null,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<FieldErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.Errors
            .Should().HaveCount(1)
            .And.Contain(x =>
                x.Messages.Single() == "'Bilanz' darf nicht leer sein, wenn 'Abgeschlossen' angegeben ist." &&
                x.Identifier == "Balance");
    }

    [Fact]
    public async Task A_closed_trade_cant_be_added_if_the_closed_date_is_missing()
    {
        // arrange
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        // act
        var act = () => client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = _utcNow,
            Closed = null,
            Size = 5000,
            Balance = 50,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<FieldErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.Errors
            .Should().HaveCount(1)
            .And.Contain(x =>
                x.Messages.Single() == "'Abgeschlossen' darf nicht leer sein, wenn 'Bilanz' angegeben ist." &&
                x.Identifier == "Closed");
    }

    [Fact]
    public async Task A_trade_with_opened_date_in_utc_will_be_stored_like_that_there_is_no_implicit_conversion()
    {
        // arrange
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        var opened = DateTimeOffset.Parse("2024-08-05T12:00:00Z");

        // act
        var response = await client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = opened,
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        response.Should().NotBeNull();
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);

        newlyAddedTrade.Should().NotBeNull();
        var expected = DateTime.Parse("2024-08-05T12:00:00");
        expected.Kind.Should().NotBe(DateTimeKind.Local);
        newlyAddedTrade!.Opened.Should().Be(expected);
    }


    [Fact]
    public async Task A_trade_with_opened_date_in_local_time_will_be_stored_as_utc_there_is_no_implicit_conversion()
    {
        // arrange
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        var openedInNewYork = DateTimeOffset.Parse("2024-08-05T12:00:00-04:00");

        // act
        var response = await client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = openedInNewYork,
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        response.Should().NotBeNull();
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);

        newlyAddedTrade.Should().NotBeNull();
        var expectedOpenedDate = DateTime.Parse("2024-08-05T16:00:00");
        expectedOpenedDate.Kind.Should().NotBe(DateTimeKind.Local);
        newlyAddedTrade!.Opened.Should().Be(expectedOpenedDate);
    }
}