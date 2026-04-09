using Microsoft.AspNetCore.Http;
using SimpleTrading.Client;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.TradesController;

public class SearchTradesTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task Search_for_trades_greater_than_opened_date_returns_correct_trades()
    {
        var client = await CreateClient();

        var initialOpenedDate = DateTime.Parse("2024-08-19T10:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        const string searchFilter = "Opened -gt [2024-08-19T11:00Z]";

        var result = await client.SearchTradesAsync(profile.Id, ["opened"], [searchFilter],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        var trade = Assert.Single(result.Data);
        Assert.Equal(DateTimeOffset.Parse("2024-08-19T14:00:00+02:00"), trade.Opened);
    }

    [Fact]
    public async Task Filter_operator_without_dash_is_not_valid()
    {
        var client = await CreateClient();
        var profile = TestData.Profile.Default.Build();
        const string searchFilter = "Opened gt [2024-08-19T11:00Z]";

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<PageDtoOfTradeDto> Act()
        {
            return client.SearchTradesAsync(profile.Id, [], [searchFilter]);
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("filter[0]", error.Key);
        Assert.Equal("Ungültiges Filterformat.", Assert.Single(error.Value));
    }

    [Fact]
    public async Task Filter_without_property_is_not_valid()
    {
        var client = await CreateClient();
        var profile = TestData.Profile.Default.Build();
        const string searchFilter = "-gt [2024-08-19T11:00Z]";

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<PageDtoOfTradeDto> Act()
        {
            return client.SearchTradesAsync(profile.Id, [], [searchFilter]);
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("filter[0]", error.Key);
        Assert.Equal("Ungültiges Filterformat.", Assert.Single(error.Value));
    }

    [Fact]
    public async Task Trades_always_belong_to_a_profile_therefore_you_cant_search_for_trades_without_a_profile_id()
    {
        var client = await CreateClient();

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<PageDtoOfTradeDto> Act()
        {
            return client.SearchTradesAsync(null, [], []);
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("profileId", error.Key);
        Assert.Equal("'Profil' darf nicht leer sein.", Assert.Single(error.Value));
    }

    [Fact]
    public async Task ProfitLoss_filter_with_date_time_as_comparison_value_returns_unprocessable_entity()
    {
        var client = await CreateClient();
        var profile = TestData.Profile.Default.Build();
        const string searchFilter = "ProfitLoss -gt [2024-08-19T11:00Z]";

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<PageDtoOfTradeDto> Act()
        {
            return client.SearchTradesAsync(profile.Id, [], [searchFilter]);
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("filter[0].ComparisonValue", error.Key);
        Assert.Equal("'2024-08-19T11:00Z' ist nicht zulässig.", Assert.Single(error.Value));
    }

    [Fact]
    public async Task ProfitLoss_filter_with_a_comparison_value_that_does_not_contain_brackets_is_not_valid()
    {
        var client = await CreateClient();
        var profile = TestData.Profile.Default.Build();
        const string searchFilter = "ProfitLoss -gt 500";

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<PageDtoOfTradeDto> Act()
        {
            return client.SearchTradesAsync(profile.Id, [], [searchFilter]);
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("filter[0]", error.Key);
        Assert.Equal("Ungültiges Filterformat.", Assert.Single(error.Value));
    }

    [Fact]
    public async Task A_filter_can_contain_multiple_whitespaces()
    {
        var client = await CreateClient();
        var now = DateTime.Parse("2024-09-22T10:00:00").ToUtcKind();

        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 3)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = now,
                Closed = now,
                ProfitLoss = 500m * x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        const string searchFilter = "    ProfitLoss   -gt   [500]    ";

        var result = await client.SearchTradesAsync(profile.Id, [], [searchFilter],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task IsClosed_does_not_take_a_null_literal_as_comparision_value()
    {
        var client = await CreateClient();
        var profile = TestData.Profile.Default.Build();
        const string searchFilter = "IsClosed -eq null";

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<PageDtoOfTradeDto> Act()
        {
            return client.SearchTradesAsync(profile.Id, [], [searchFilter]);
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ValidationProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, exception.StatusCode);
        var error = Assert.Single(exception.Result.Errors);
        Assert.Equal("filter[0].ComparisonValue", error.Key);
        Assert.Equal("Literal 'null' ist hier nicht erlaubt.", Assert.Single(error.Value));
    }

    [Fact]
    public async Task IsClosed_property_filter_accepts_true_literal()
    {
        var client = await CreateClient();
        var now = DateTime.Parse("2024-09-22T10:00:00").ToUtcKind();

        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 3)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = now,
                Closed = now,
                ProfitLoss = 500m
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        const string searchFilter = "IsClosed -eq true";

        var result = await client.SearchTradesAsync(profile.Id, [], [searchFilter],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task IsClosed_equals_false_returns_only_open_trades()
    {
        var client = await CreateClient();
        var now = DateTime.Parse("2024-09-22T10:00:00").ToUtcKind();

        var profile = TestData.Profile.Default.Build();
        var openedTrade = TestData.Trade.Default with
        {
            ProfileOrId = profile,
            Opened = now
        };

        var closedTrade = TestData.Trade.Default with
        {
            ProfileOrId = profile,
            Opened = now,
            Closed = now,
            ProfitLoss = 500m
        };

        DbContext.Profiles.Add(profile);
        DbContext.Trades.AddRange(openedTrade.Build(), closedTrade.Build());
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        const string searchFilter = "IsClosed -eq false";

        var result = await client.SearchTradesAsync(profile.Id, [], [searchFilter],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(1, result.Count);
        var returnedOpenedTrade = result.Data.First();
        Assert.Equal(openedTrade.Id, returnedOpenedTrade.Id);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("NULL")]
    [InlineData("nuLL")]
    [InlineData("nUlL")]
    [InlineData("null ")]
    [InlineData(" null")]
    public async Task A_filter_with_null_literal_in_different_casing_and_whitespaces_is_totally_fine(string nullLiteral)
    {
        var client = await CreateClient();

        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 3)
            .Select(_ => TestData.Trade.Default with {ProfileOrId = profile})
            .Select(x => x.Build())
            .ToList();

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var searchFilter = $"Closed -eq {nullLiteral}";

        var result = await client.SearchTradesAsync(profile.Id, [], [searchFilter],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task Filtering_for_trades_that_have_a_closed_date_returns_nothing_when_there_are_no_closed_trades()
    {
        var client = await CreateClient();
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 3)
            .Select(_ => TestData.Trade.Default with {ProfileOrId = profile})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        const string searchFilter = "Closed -ne null";

        var result = await client.SearchTradesAsync(profile.Id, [], [searchFilter],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(0, result.Count);
    }

    [Fact]
    public async Task Null_as_filter_is_being_ignored()
    {
        var client = await CreateClient();
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 3)
            .Select(_ => TestData.Trade.Default with {ProfileOrId = profile})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        const string searchFilter = null!;

        var result = await client.SearchTradesAsync(profile.Id, [], [searchFilter],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task Null_as_sort_is_being_ignored()
    {
        var client = await CreateClient();

        var openedClosedDate = DateTime.Parse("2024-08-19T19:30:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 3)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedClosedDate,
                Closed = openedClosedDate,
                ProfitLoss = 500m * x,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        List<string> sorting = ["-Result", null!];

        var result = await client.SearchTradesAsync(profile.Id, sorting, [],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(3, result.Count);
        Assert.Equal(ResultDto.Mediocre, result.Data.ElementAt(0).Result!.Value);
        Assert.Equal(ResultDto.BreakEven, result.Data.ElementAt(1).Result!.Value);
        Assert.Equal(ResultDto.Loss, result.Data.ElementAt(2).Result!.Value);
    }
}