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
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<AddTradeResultDto> Act() => client.AddTradeAsync();

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException>(Act);
        Assert.Equal(StatusCodes.Status401Unauthorized, exception.StatusCode);
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
        Assert.NotNull(response);
        Assert.Empty(response.Warnings);
        Assert.NotNull(response);
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);
        Assert.NotNull(newlyAddedTrade);
    }
    
    [Fact]
    public async Task A_trade_is_not_saved_when_executing_a_dry_run()
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
            DryRun = true,
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = _utcNow,
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        });

        // assert
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);
        Assert.Null(newlyAddedTrade);
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
        Assert.NotNull(response);
        Assert.Empty(response.Warnings);
        Assert.NotNull(response);
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);
        Assert.NotNull(newlyAddedTrade);
        Assert.Null(newlyAddedTrade.Result);
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
        Assert.NotNull(response);
        Assert.NotNull(response);
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);
        Assert.NotNull(newlyAddedTrade);
        Assert.NotNull(newlyAddedTrade.Result);
        Assert.Equal(Result.Loss, newlyAddedTrade.Result!.Name);
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
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<AddTradeResultDto> Act() =>
            client.AddTradeAsync(new AddTradeDto
            {
                AssetId = asset.Id,
                ProfileId = profile.Id,
                Opened = _utcNow,
                Size = null,
                CurrencyId = currency.Id,
                EntryPrice = 1.08
            });

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<FieldErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("size", error.Identifier);
        Assert.Equal("'Handelsvolumen' darf kein Nullwert sein.", Assert.Single(error.Messages));
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
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<AddTradeResultDto> Act() =>
            client.AddTradeAsync(new AddTradeDto
            {
                AssetId = notExistingAssetId,
                ProfileId = profile.Id,
                Opened = _utcNow,
                Size = 5000,
                CurrencyId = currency.Id,
                EntryPrice = 1.08
            });

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.Equal("Asset nicht gefunden.", Assert.Single(exception.Result.Messages));
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
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<AddTradeResultDto> Act() =>
            client.AddTradeAsync(new AddTradeDto
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
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<FieldErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("balance", error.Identifier);
        Assert.Equal("'Bilanz' darf nicht leer sein, wenn 'Abgeschlossen' angegeben ist.", 
            Assert.Single(error.Messages));
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
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<AddTradeResultDto> Act() =>
            client.AddTradeAsync(new AddTradeDto
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
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<FieldErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("closed", error.Identifier);
        Assert.Equal("'Abgeschlossen' darf nicht leer sein, wenn 'Bilanz' angegeben ist.", 
            Assert.Single(error.Messages));
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
        Assert.NotNull(response);
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);
        Assert.NotNull(newlyAddedTrade);
        var expected = DateTime.Parse("2024-08-05T12:00:00");
        Assert.NotEqual(DateTimeKind.Local, expected.Kind);
        Assert.Equal(expected, newlyAddedTrade.Opened);
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
        Assert.NotNull(response);
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);
        Assert.NotNull(newlyAddedTrade);
        var expectedOpenedDate = DateTime.Parse("2024-08-05T16:00:00");
        Assert.NotEqual(DateTimeKind.Local, expectedOpenedDate.Kind);
        Assert.Equal(expectedOpenedDate, newlyAddedTrade.Opened);
    }
}