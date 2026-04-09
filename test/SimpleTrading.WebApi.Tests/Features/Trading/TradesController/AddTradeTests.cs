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
        var client = await CreateClient(false);

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<AddTradeResultDto> Act()
        {
            return client.AddTradeAsync(new AddTradeDto());
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status401Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task A_trade_can_be_successfully_added()
    {
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = _utcNow,
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        }, false, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        Assert.Empty(response.Warnings);
        Assert.NotNull(response);
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);
        Assert.NotNull(newlyAddedTrade);
    }

    [Fact]
    public async Task A_trade_is_not_saved_when_executing_a_dry_run()
    {
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = _utcNow,
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        }, true, TestContext.Current.CancellationToken);

        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);
        Assert.Null(newlyAddedTrade);
    }

    [Fact]
    public async Task A_closed_trade_with_an_overriden_null_result_will_be_added()
    {
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = _utcNow,
            Closed = _utcNow,
            ProfitLoss = 0,
            ManuallyEnteredResult = new UpdateResultValue {Value = null},
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        }, false, TestContext.Current.CancellationToken);

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
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = _utcNow,
            Closed = _utcNow,
            ProfitLoss = 0,
            ManuallyEnteredResult = new UpdateResultValue {Value = ResultDto.Loss},
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        }, false, TestContext.Current.CancellationToken);

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
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<AddTradeResultDto> Act()
        {
            return client.AddTradeAsync(new AddTradeDto
            {
                AssetId = asset.Id,
                ProfileId = profile.Id,
                Opened = _utcNow,
                Size = null,
                CurrencyId = currency.Id,
                EntryPrice = 1.08
            });
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("size", error.Key);
        Assert.Equal("'Handelsvolumen' darf kein Nullwert sein.", Assert.Single(error.Value));
    }

    [Fact]
    public async Task A_trade_cant_be_added_if_the_asset_is_missing()
    {
        var client = await CreateClient();

        var notExistingAssetId = Guid.Parse("a622d632-a7ef-42fe-adfa-fcb917e65926");
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<AddTradeResultDto> Act()
        {
            return client.AddTradeAsync(new AddTradeDto
            {
                AssetId = notExistingAssetId,
                ProfileId = profile.Id,
                Opened = _utcNow,
                Size = 5000,
                CurrencyId = currency.Id,
                EntryPrice = 1.08
            });
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, exception.StatusCode);
        var (identifier, errors) = Assert.Single(exception.Result.Errors);
        Assert.Equal("assetId", identifier);
        Assert.Equal("Asset nicht gefunden.", Assert.Single(errors));
    }

    [Fact]
    public async Task A_closed_trade_cant_be_added_if_the_profitLoss_is_missing()
    {
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<AddTradeResultDto> Act()
        {
            return client.AddTradeAsync(new AddTradeDto
            {
                AssetId = asset.Id,
                ProfileId = profile.Id,
                Opened = _utcNow,
                Closed = _utcNow,
                ManuallyEnteredResult = null,
                Size = 5000,
                ProfitLoss = null,
                CurrencyId = currency.Id,
                EntryPrice = 1.08
            });
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("profitLoss", error.Key);
        Assert.Equal("'Gewinn/Verlust' darf nicht leer sein, wenn 'Abgeschlossen' angegeben ist.",
            Assert.Single(error.Value));
    }

    [Fact]
    public async Task A_closed_trade_cant_be_added_if_the_closed_date_is_missing()
    {
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<AddTradeResultDto> Act()
        {
            return client.AddTradeAsync(new AddTradeDto
            {
                AssetId = asset.Id,
                ProfileId = profile.Id,
                Opened = _utcNow,
                Closed = null,
                Size = 5000,
                ProfitLoss = 50,
                CurrencyId = currency.Id,
                EntryPrice = 1.08
            });
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("closed", error.Key);
        Assert.Equal("'Abgeschlossen' darf nicht leer sein, wenn 'Gewinn/Verlust' angegeben ist.",
            Assert.Single(error.Value));
    }

    [Fact]
    public async Task A_trade_with_opened_date_in_utc_will_be_stored_like_that_there_is_no_implicit_conversion()
    {
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var opened = DateTimeOffset.Parse("2024-08-05T12:00:00Z");

        var response = await client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = opened,
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        }, false, TestContext.Current.CancellationToken);

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
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var openedInNewYork = DateTimeOffset.Parse("2024-08-05T12:00:00-04:00");

        var response = await client.AddTradeAsync(new AddTradeDto
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = openedInNewYork,
            Size = 5000,
            CurrencyId = currency.Id,
            EntryPrice = 1.08
        }, false, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == response.TradeId);
        Assert.NotNull(newlyAddedTrade);
        var expectedOpenedDate = DateTime.Parse("2024-08-05T16:00:00");
        Assert.NotEqual(DateTimeKind.Local, expectedOpenedDate.Kind);
        Assert.Equal(expectedOpenedDate, newlyAddedTrade.Opened);
    }

    [Fact]
    public async Task A_trade_reference_with_an_invalid_links_returns_a_bad_request()
    {
        var client = await CreateClient();

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<AddTradeResultDto> Act()
        {
            return client.AddTradeAsync(new AddTradeDto
            {
                AssetId = asset.Id,
                ProfileId = profile.Id,
                Opened = _utcNow,
                Closed = null,
                Size = 5000,
                ProfitLoss = 50,
                CurrencyId = currency.Id,
                EntryPrice = 1.08,
                References =
                [
                    new AddReferenceDto
                    {
                        Type = ReferenceTypeDto.Other,
                        Link = "bad url"
                    }
                ]
            });
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("references[0].Link", error.Key);
        Assert.Equal("Ungültiger Link.",
            Assert.Single(error.Value));
    }
}