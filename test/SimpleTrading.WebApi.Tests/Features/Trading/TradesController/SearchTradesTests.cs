﻿using AwesomeAssertions;
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
        // arrange
        var client = await CreateClient();

        var initialOpenedDate = DateTime.Parse("2024-08-19T10:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync();

        const string searchFilter = "Opened -gt [2024-08-19T11:00Z]";

        // act
        var result = await client.SearchTradesAsync(profile.Id, ["opened"], [searchFilter]);

        // assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result.Data.Should().HaveCount(1)
            .And.Contain(x => x.Opened!.Value == DateTimeOffset.Parse("2024-08-19T14:00:00+02:00"));
    }

    [Fact]
    public async Task Filter_operator_without_dash_is_not_valid()
    {
        // arrange
        var client = await CreateClient();
        var profile = TestData.Profile.Default.Build();
        const string searchFilter = "Opened gt [2024-08-19T11:00Z]";

        // act
        var act = () => client.SearchTradesAsync(profile.Id, [], [searchFilter]);

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<FieldErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.Errors.Should().HaveCount(1)
            .And.Contain(x => x.Messages.Single() == "Ungültiges Filterformat." &&
                              x.Identifier == "Filter[0]");
    }

    [Fact]
    public async Task Filter_without_property_is_not_valid()
    {
        // arrange
        var client = await CreateClient();
        var profile = TestData.Profile.Default.Build();
        const string searchFilter = "-gt [2024-08-19T11:00Z]";

        // act
        var act = () => client.SearchTradesAsync(profile.Id, [], [searchFilter]);

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<FieldErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.Errors.Should().HaveCount(1)
            .And.Contain(x => x.Messages.Single() == "Ungültiges Filterformat." &&
                              x.Identifier == "Filter[0]");
    }
    
    [Fact]
    public async Task Trades_always_belong_to_a_profile_therefore_you_cant_search_for_trades_without_a_profile_id()
    {
        // arrange
        var client = await CreateClient();

        // act
        var act = () => client.SearchTradesAsync(null, [], []);

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<FieldErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.Errors.Should().HaveCount(1)
            .And.Contain(x => x.Messages.Single() == "'Profil' darf nicht leer sein." &&
                              x.Identifier == "ProfileId");
    }

    [Fact]
    public async Task Balance_filter_with_date_time_as_comparison_value_returns_a_bad_request()
    {
        // arrange
        var client = await CreateClient();
        var profile = TestData.Profile.Default.Build();
        const string searchFilter = "Balance -gt [2024-08-19T11:00Z]";

        // act
        var act = () => client.SearchTradesAsync(profile.Id, [], [searchFilter]);

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<FieldErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.Errors.Should().HaveCount(1)
            .And.Contain(x => x.Messages.Single() == "'2024-08-19T11:00Z' ist nicht zulässig." &&
                              x.Identifier == "Filter[0].ComparisonValue");
    }

    [Fact]
    public async Task Balance_filter_with_a_comparison_value_that_does_not_contain_brackets_is_not_valid()
    {
        // arrange
        var client = await CreateClient();
        var profile = TestData.Profile.Default.Build();
        const string searchFilter = "Balance -gt 500";

        // act
        var act = () => client.SearchTradesAsync(profile.Id, [], [searchFilter]);

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<FieldErrorResponse>>();
        exception.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Which.Result.Errors.Should().HaveCount(1)
            .And.Contain(x => x.Messages.Single() == "Ungültiges Filterformat." &&
                              x.Identifier == "Filter[0]");
    }

    [Fact]
    public async Task A_filter_can_contain_multiple_whitespaces()
    {
        // arrange
        var client = await CreateClient();
        var now = DateTime.Parse("2024-09-22T10:00:00").ToUtcKind();

        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 3)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = now,
                Closed = now,
                Balance = 500m * x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync();

        const string searchFilter = "    Balance   -gt   [500]    ";

        // act
        var result = await client.SearchTradesAsync(profile.Id, [], [searchFilter]);

        // assert
        result.Count.Should().Be(2);
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
        // arrange
        var client = await CreateClient();

        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 3)
            .Select(_ => TestData.Trade.Default with {ProfileOrId = profile})
            .Select(x => x.Build())
            .ToList();

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync();

        var searchFilter = $"Closed -eq {nullLiteral}";

        // act
        var result = await client.SearchTradesAsync(profile.Id, [], [searchFilter]);

        // assert
        result.Count.Should().Be(3);
    }

    [Fact]
    public async Task A_filter_for_trades_that_dont_have_a_closed_value_return_correct_result()
    {
        // arrange
        var client = await CreateClient();
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 3)
            .Select(_ => TestData.Trade.Default with {ProfileOrId = profile})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync();

        const string searchFilter = "Closed -ne null";

        // act
        var result = await client.SearchTradesAsync(profile.Id, [], [searchFilter]);

        // assert
        result.Count.Should().Be(0);
    }

    [Fact]
    public async Task Null_as_filter_is_being_ignored()
    {
        // arrange
        var client = await CreateClient();
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 3)
            .Select(_ => TestData.Trade.Default with {ProfileOrId = profile})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync();

        const string searchFilter = null!;

        // act
        var result = await client.SearchTradesAsync(profile.Id, [], [searchFilter]);

        // assert
        result.Count.Should().Be(3);
    }

    [Fact]
    public async Task Null_as_sort_is_being_ignored()
    {
        // arrange
        var client = await CreateClient();

        var openedClosedDate = DateTime.Parse("2024-08-19T19:30:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 3)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedClosedDate,
                Closed = openedClosedDate,
                Balance = 500m * x,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync();

        List<string> sorting = ["-Result", null!];

        // act
        var result = await client.SearchTradesAsync(profile.Id, sorting, []);

        // assert
        result.Count.Should().Be(3);
        result.Data.ElementAt(0).Result!.Value.Should().Be(ResultDto.Mediocre);
        result.Data.ElementAt(1).Result!.Value.Should().Be(ResultDto.BreakEven);
        result.Data.ElementAt(2).Result!.Value.Should().Be(ResultDto.Loss);
    }
}