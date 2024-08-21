using FluentAssertions;
using SimpleTrading.Client;
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
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        const string searchFilter = "Opened -gt [2024-08-19T11:00Z]";

        // act
        var result = await client.SearchTradesAsync(["opened"], [searchFilter]);

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

        const string searchFilter = "Opened gt [2024-08-19T11:00Z]";

        // act
        var act = () => client.SearchTradesAsync([], [searchFilter]);

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.Result.FieldErrors.Should().HaveCount(1)
            .And.Contain(x => x.Messages.Single() == "'Filter' weist ein ungültiges Format auf." &&
                              x.Identifier == "Filter[0].Filter");
    }

    [Fact]
    public async Task Filter_without_property_is_not_valid()
    {
        // arrange
        var client = await CreateClient();

        const string searchFilter = "-gt [2024-08-19T11:00Z]";

        // act
        var act = () => client.SearchTradesAsync([], [searchFilter]);

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.Result.FieldErrors.Should().HaveCount(1)
            .And.Contain(x => x.Messages.Single() == "'Filter' weist ein ungültiges Format auf." &&
                              x.Identifier == "Filter[0].Filter");
    }

    [Fact]
    public async Task Balance_filter_with_date_time_as_comparison_value_returns_a_bad_request()
    {
        // arrange
        var client = await CreateClient();

        const string searchFilter = "Balance -gt [2024-08-19T11:00Z]";

        // act
        var act = () => client.SearchTradesAsync([], [searchFilter]);

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.Result.FieldErrors.Should().HaveCount(1)
            .And.Contain(x => x.Messages.Single() == "'2024-08-19T11:00Z' ist nicht zulässig." &&
                              x.Identifier == "Filter[0].ComparisonValue");
    }

    [Fact]
    public async Task Balance_filter_with_a_comparison_value_that_does_not_contain_brackets_is_not_valid()
    {
        // arrange
        var client = await CreateClient();

        const string searchFilter = "Balance -gt 500";

        // act
        var act = () => client.SearchTradesAsync([], [searchFilter]);

        // assert
        var exception = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        exception.Which.Result.FieldErrors.Should().HaveCount(1)
            .And.Contain(x => x.Messages.Single() == "'Filter' weist ein ungültiges Format auf." &&
                              x.Identifier == "Filter[0].Filter");
    }

    [Fact]
    public async Task A_filter_can_contain_multiple_whitespaces()
    {
        // arrange
        var client = await CreateClient();

        var trades = Enumerable.Range(1, 3)
            .Select(x => TestData.Trade.Default with {Balance = 500m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        const string searchFilter = "    Balance   -gt   [500]    ";

        // act
        var result = await client.SearchTradesAsync([], [searchFilter]);

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

        var trades = Enumerable.Range(1, 3)
            .Select(x => TestData.Trade.Default)
            .Select(x => x.Build())
            .ToList();

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var searchFilter = $"Closed -eq {nullLiteral}";

        // act
        var result = await client.SearchTradesAsync([], [searchFilter]);

        // assert
        result.Count.Should().Be(3);
    }

    [Fact]
    public async Task A_filter_for_trades_that_dont_have_a_closed_value_return_correct_result()
    {
        // arrange
        var client = await CreateClient();

        var trades = Enumerable.Range(1, 3)
            .Select(x => TestData.Trade.Default)
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        const string searchFilter = "Closed -ne null";

        // act
        var result = await client.SearchTradesAsync([], [searchFilter]);

        // assert
        result.Count.Should().Be(0);
    }

    [Fact]
    public async Task Null_as_filter_is_being_ignored()
    {
        // arrange
        var client = await CreateClient();

        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default)
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        const string searchFilter = null!;

        // act
        var result = await client.SearchTradesAsync([], [searchFilter]);

        // assert
        result.Count.Should().Be(3);
    }

    [Fact]
    public async Task Null_as_sort_is_being_ignored()
    {
        // arrange
        var client = await CreateClient();

        var openedClosedDate = DateTime.Parse("2024-08-19T19:30:00");
        var trades = Enumerable.Range(1, 3)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosedDate,
                Closed = openedClosedDate,
                Balance = 500m * x,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        List<string> sorting = ["-Result", null!];

        // act
        var result = await client.SearchTradesAsync(sorting, []);

        // assert
        result.Count.Should().Be(3);
        result.Data.ElementAt(0).Result!.Value.Should().Be(ResultDto.Mediocre);
        result.Data.ElementAt(1).Result!.Value.Should().Be(ResultDto.BreakEven);
        result.Data.ElementAt(2).Result!.Value.Should().Be(ResultDto.Loss);
    }
}