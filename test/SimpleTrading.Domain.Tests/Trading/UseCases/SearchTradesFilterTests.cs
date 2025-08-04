using Autofac;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class SearchTradesFilterTests : DomainTests
{
    private ISearchTrades Interactor => ServiceLocator.Resolve<ISearchTrades>();

    [Fact]
    public async Task Greater_than_opened_date_with_comparison_value_in_local_time_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Profiles.Add(profile);
        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "gt",
            ComparisonValue = "2024-08-19T17:00:00+02:00",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var expected = DateTimeOffset.Parse("2024-08-19T18:00:00+02:00");
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);

        var trade = Assert.Single(pagedTrades);
        Assert.Equal(expected, trade.Opened);
    }

    [Fact]
    public async Task Greater_than_opened_date_with_comparison_value_in_utc_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "gt",
            ComparisonValue = "2024-08-19T15:00:00Z",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var expected = DateTimeOffset.Parse("2024-08-19T18:00:00+02:00");
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        
        var trade = Assert.Single(pagedTrades);
        Assert.Equal(expected, trade.Opened);
    }

    [Fact]
    public async Task Closed_greater_than_null_returns_bad_input()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var filter = new FilterModel
        {
            PropertyName = "Closed",
            Operator = "gt",
            ComparisonValue = "null",
            IsLiteral = true
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("Null is not allowed here.", error.ErrorMessage);
    }

    [Fact]
    public async Task Greater_than_opened_date_with_invalid_comparison_value_returns_bad_input()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "gt",
            ComparisonValue = "2024-08-19T17:00:",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("'2024-08-19T17:00:' is not valid.", error.ErrorMessage);
        Assert.Equal("Filter[0].ComparisonValue", error.PropertyName);
    }

    [Fact]
    public async Task Greater_than_opened_date_with_typo_in_operator_returns_bad_input()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "grt",
            ComparisonValue = "2024-08-19T17:00:00",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("The operator 'grt' is not supported.", error.ErrorMessage);
        Assert.Equal("Filter[0].Operator", error.PropertyName);
    }


    [Fact]
    public async Task Greater_than_opened_date_with_typo_in_property_name_returns_bad_input()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var filter = new FilterModel
        {
            PropertyName = "Openend",
            Operator = "gt",
            ComparisonValue = "2024-08-19T17:00:00",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("'Openend' cannot be used as a filter.", error.ErrorMessage);
        Assert.Equal("Filter[0].PropertyName", error.PropertyName);
    }

    [Fact]
    public async Task Greater_than_or_equal_to_opened_date_with_comparison_value_in_local_time_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "ge",
            ComparisonValue = "2024-08-19T17:00:00+02:00",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var firstExpected = DateTimeOffset.Parse("2024-08-19T17:00:00+02:00");
        var secondExpected = DateTimeOffset.Parse("2024-08-19T18:00:00+02:00");

        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(2, pagedTrades.Count);
        Assert.Contains(pagedTrades, item => firstExpected == item.Opened);
        Assert.Contains(pagedTrades, item => secondExpected == item.Opened);
    }

    [Fact]
    public async Task Less_than_or_equal_to_opened_date_with_comparison_value_in_local_time_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "le",
            ComparisonValue = "2024-08-19T17:00:00+02:00",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var firstExpected = DateTimeOffset.Parse("2024-08-19T16:00:00+02:00");
        var secondExpected = DateTimeOffset.Parse("2024-08-19T17:00:00+02:00");

        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(2, pagedTrades.Count);
        Assert.Contains(pagedTrades, item => firstExpected == item.Opened);
        Assert.Contains(pagedTrades, item => secondExpected == item.Opened);
    }

    [Fact]
    public async Task Less_than_opened_date_with_comparison_value_in_local_time_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "lt",
            ComparisonValue = "2024-08-19T17:00:00+02:00",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var expected = DateTimeOffset.Parse("2024-08-19T16:00:00+02:00");
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        
        var singleTrade = Assert.Single(pagedTrades);
        Assert.Equal(expected, singleTrade.Opened);
    }

    [Fact]
    public async Task Equal_to_opened_date_with_comparison_value_in_local_time_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "eq",
            ComparisonValue = "2024-08-19T17:00:00+02:00",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var expected = DateTimeOffset.Parse("2024-08-19T17:00:00+02:00");
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);

        var singleTrade = Assert.Single(pagedTrades);
        Assert.Equal(expected, singleTrade.Opened);
    }

    [Fact]
    public async Task Not_equal_to_opened_date_with_comparison_value_in_local_time_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "ne",
            ComparisonValue = "2024-08-19T17:00:00+02:00",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var firstExpected = DateTimeOffset.Parse("2024-08-19T16:00:00+02:00");
        var secondExpected = DateTimeOffset.Parse("2024-08-19T18:00:00+02:00");
        
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(2, pagedTrades.Count);
        Assert.Contains(pagedTrades, item => firstExpected == item.Opened);
        Assert.Contains(pagedTrades, item => secondExpected == item.Opened);
    }

    [Fact]
    public async Task Greater_than_profitLoss_with_uppercase_property_name_returns_correct_result()
    {
        // arrange
        var now = DateTime.Parse("2024-09-22T10:00:00").ToUtcKind();
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 2)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = now,
                Closed = now,
                ProfitLoss = 500m * x
            })
            .Select(x => x.Build());

        DbContext.Profiles.Add(profile);
        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "PROFITLOSS",
            Operator = "gt",
            ComparisonValue = "500",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        
        var singleTrade = Assert.Single(pagedTrades);
        Assert.Equal(1000m, singleTrade.ProfitLoss);
    }

    [Fact]
    public async Task Greater_than_profitLoss_with_empty_comparison_value_returns_bad_input()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var filter = new FilterModel
        {
            PropertyName = "ProfitLoss",
            Operator = "gt",
            ComparisonValue = "",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("'Comparison value' must not be empty.", error.ErrorMessage);
        Assert.Equal("Filter[0].ComparisonValue", error.PropertyName);
    }

    [Fact]
    public async Task Greater_than_profitLoss_with_empty_operator_returns_bad_input()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var filter = new FilterModel
        {
            PropertyName = "ProfitLoss",
            Operator = "",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("'Operator' must not be empty.", error.ErrorMessage);
        Assert.Equal("Filter[0].Operator", error.PropertyName);
    }

    [Fact]
    public async Task Greater_than_profitLoss_with_empty_property_name_returns_bad_input()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var filter = new FilterModel
        {
            PropertyName = "",
            Operator = "gt",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("'Field' must not be empty.", error.ErrorMessage);
        Assert.Equal("Filter[0].PropertyName", error.PropertyName);
    }

    [Fact]
    public async Task Unknown_property_names_cannot_be_used_as_a_filter()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var filter = new FilterModel
        {
            PropertyName = "Foobar",
            Operator = "gt",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("'Foobar' cannot be used as a filter.", error.ErrorMessage);
        Assert.Equal("Filter[0].PropertyName", error.PropertyName);
    }

    [Fact]
    public async Task An_unknown_operator_cannot_be_used_in_a_filter()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var filter = new FilterModel
        {
            PropertyName = "ProfitLoss",
            Operator = "gr",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("The operator 'gr' is not supported.", error.ErrorMessage);
        Assert.Equal("Filter[0].Operator", error.PropertyName);
    }

    [Fact]
    public async Task A_null_property_name_returns_bad_input()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var filter = new FilterModel
        {
            PropertyName = null!,
            Operator = "gt",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("'Field' must not be empty.", error.ErrorMessage);
        Assert.Equal("Filter[0].PropertyName", error.PropertyName);
    }

    [Fact]
    public async Task Property_names_with_whitespaces_will_not_be_trimmed_and_bad_input_gets_returned()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var filter = new FilterModel
        {
            PropertyName = "profitLoss ",
            Operator = "gt",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("'profitLoss ' cannot be used as a filter.", error.ErrorMessage);
        Assert.Equal("Filter[0].PropertyName", error.PropertyName);
    }

    [Fact]
    public async Task Operators_with_whitespaces_will_not_be_trimmed_and_bad_input_gets_returned()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var filter = new FilterModel
        {
            PropertyName = "profitLoss",
            Operator = " gt",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("The operator ' gt' is not supported.", error.ErrorMessage);
        Assert.Equal("Filter[0].Operator", error.PropertyName);
    }

    [Fact]
    public async Task Greater_than_or_equal_to_size_with_valid_input_returns_correct_result()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 2)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Size = 5000m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Size",
            Operator = "ge",
            ComparisonValue = "10000",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        var singleTrade = Assert.Single(pagedTrades);
        Assert.Equal(10_000m, singleTrade.Size);
    }

    [Fact]
    public async Task Uppercase_operator_works_as_well()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 2)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Size = 5000m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Size",
            Operator = "GE",
            ComparisonValue = "10000",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        var singleTrade = Assert.Single(pagedTrades);
        Assert.Equal(10_000m, singleTrade.Size);
    }

    [Fact]
    public async Task Less_than_or_equal_to_size_with_valid_input_returns_correct_result()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 2)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Size = 5000m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Size",
            Operator = "le",
            ComparisonValue = "5000",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        var singleTrade = Assert.Single(pagedTrades);
        Assert.Equal(5_000m, singleTrade.Size);
    }

    [Fact]
    public async Task Not_equal_to_size_with_valid_input_returns_correct_result()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 2)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Size = 5000m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Size",
            Operator = "ne",
            ComparisonValue = "5000",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        var singleTrade = Assert.Single(pagedTrades);
        Assert.Equal(10_000m, singleTrade.Size);
    }

    [Fact]
    public async Task Less_than_profitLoss_with_valid_input_returns_correct_result()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 2)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, ProfitLoss = 500m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "ProfitLoss",
            Operator = "lt",
            ComparisonValue = "500",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Empty(pagedTrades);
    }

    [Fact]
    public async Task Equal_to_profitLoss_with_valid_input_returns_correct_result()
    {
        // arrange
        var now = DateTime.Parse("2024-09-22T10:00:00").ToUtcKind();
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(1, 2)
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

        var filter = new FilterModel
        {
            PropertyName = "ProfitLoss",
            Operator = "eq",
            ComparisonValue = "500",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        var singleTrade = Assert.Single(pagedTrades);
        Assert.Equal(500m, singleTrade.ProfitLoss);
    }

    [Fact]
    public async Task ProfitLoss_equal_to_null_returns_trades_without_a_profitLoss()
    {
        // arrange
        var now = DateTime.Parse("2024-09-22T10:00:00").ToUtcKind();
        var profile = TestData.Profile.Default.Build();
        var tradeWithProfitLoss = (TestData.Trade.Default with
        {
            ProfileOrId = profile,
            Opened = now,
            Closed = now,
            ProfitLoss = 500m
        }).Build();
        var tradeWithoutProfitLoss = (TestData.Trade.Default with {ProfileOrId = profile}).Build();

        DbContext.Trades.AddRange(tradeWithProfitLoss, tradeWithoutProfitLoss);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "ProfitLoss",
            Operator = "eq",
            ComparisonValue = "null",
            IsLiteral = true
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        var singleTrade = Assert.Single(pagedTrades);
        Assert.Equal(tradeWithoutProfitLoss.Id, singleTrade.Id);
    }

    [Fact]
    public async Task Greater_than_result_with_invalid_comparison_value_returns_bad_input()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Result = (ResultModel) x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "gt",
            ComparisonValue = "NotThatBad",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("'NotThatBad' is not valid.", error.ErrorMessage);
        Assert.Equal("Filter[0].ComparisonValue", error.PropertyName);
    }

    [Fact]
    public async Task Greater_than_result_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedClosed,
                Closed = openedClosed,
                ProfitLoss = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "gt",
            ComparisonValue = "BreakEven",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(2, pagedTrades.Count);
        Assert.Contains(pagedTrades, item => ResultModel.Mediocre == item.Result);
        Assert.Contains(pagedTrades, item => ResultModel.Win == item.Result);
    }


    [Fact]
    public async Task Greater_than_closed_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedClosed.AddHours(x),
                Closed = openedClosed.AddHours(x),
                ProfitLoss = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Closed",
            Operator = "gt",
            ComparisonValue = "2024-08-19T15:00:00Z",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(3, pagedTrades.Count);
        Assert.DoesNotContain(pagedTrades, item => item.Closed!.Value.UtcDateTime == openedClosed);
    }

    [Fact]
    public async Task Greater_than_or_equal_to_result_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedClosed,
                Closed = openedClosed,
                ProfitLoss = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "ge",
            ComparisonValue = "BreakEven",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(3, pagedTrades.Count);
        Assert.Contains(pagedTrades, item => ResultModel.BreakEven == item.Result);
        Assert.Contains(pagedTrades, item => ResultModel.Mediocre == item.Result);
        Assert.Contains(pagedTrades, item => ResultModel.Win == item.Result);
    }


    [Fact]
    public async Task Result_greater_than_or_equal_to_null_returns_bad_input()
    {
        // arrange
        var profile = TestData.Profile.Default.Build();
        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "ge",
            ComparisonValue = "null",
            IsLiteral = true
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var badInput = Assert.IsType<BadInput>(response.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("Null is not allowed here.", error.ErrorMessage);
        Assert.Equal("Filter[0].ComparisonValue", error.PropertyName);
    }

    [Fact]
    public async Task Less_than_or_equal_to_result_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedClosed,
                Closed = openedClosed,
                ProfitLoss = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "le",
            ComparisonValue = "BreakEven",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(2, pagedTrades.Count);
        Assert.Contains(pagedTrades, item => ResultModel.BreakEven == item.Result);
        Assert.Contains(pagedTrades, item => ResultModel.Loss == item.Result);
    }

    [Fact]
    public async Task Less_than_to_result_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedClosed,
                Closed = openedClosed,
                ProfitLoss = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "lt",
            ComparisonValue = "Mediocre",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(2, pagedTrades.Count);
        Assert.Contains(pagedTrades, item => ResultModel.BreakEven == item.Result);
        Assert.Contains(pagedTrades, item => ResultModel.Loss == item.Result);
    }

    [Fact]
    public async Task Equal_to_result_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedClosed,
                Closed = openedClosed,
                ProfitLoss = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "eq",
            ComparisonValue = "Mediocre",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        var singleTrade = Assert.Single(pagedTrades);
        Assert.Equal(ResultModel.Mediocre,  singleTrade.Result);
    }

    [Fact]
    public async Task Not_equal_to_result_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedClosed,
                Closed = openedClosed,
                ProfitLoss = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "ne",
            ComparisonValue = "Mediocre",
            IsLiteral = false
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(3, pagedTrades.Count);
        Assert.Contains(pagedTrades, item => ResultModel.Loss == item.Result);
        Assert.Contains(pagedTrades, item => ResultModel.BreakEven == item.Result);
        Assert.Contains(pagedTrades, item => ResultModel.Win == item.Result);
    }

    [Fact]
    public async Task Equal_to_null_result_returns_all_trades_without_a_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var profile = TestData.Profile.Default.Build();
        var tradesWithoutResult = Enumerable.Range(0, 2)
            .Select(_ => TestData.Trade.Default with {ProfileOrId = profile})
            .Select(x => x.Build())
            .ToList();

        var tradesWithResult = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedClosed,
                Closed = openedClosed,
                ProfitLoss = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build())
            .ToList();

        DbContext.Trades.AddRange(tradesWithoutResult.Concat(tradesWithResult));
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "eq",
            ComparisonValue = "null",
            IsLiteral = true
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(2, pagedTrades.Count);
        Assert.Contains(pagedTrades, item => tradesWithoutResult[0].Id == item.Id);
        Assert.Contains(pagedTrades, item => tradesWithoutResult[1].Id == item.Id);
    }

    [Fact]
    public async Task Result_not_equal_to_null_returns_all_trades_with_a_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var profile = TestData.Profile.Default.Build();
        var tradesWithoutResult = Enumerable.Range(0, 2)
            .Select(_ => TestData.Trade.Default with {ProfileOrId = profile})
            .Select(x => x.Build())
            .ToList();

        var tradesWithResult = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosed,
                ProfileOrId = profile,
                Closed = openedClosed,
                ProfitLoss = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build())
            .ToList();

        DbContext.Trades.AddRange(tradesWithoutResult.Concat(tradesWithResult));
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "ne",
            ComparisonValue = "null",
            IsLiteral = true
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(4, pagedTrades.Count);
        Assert.Contains(pagedTrades, item => tradesWithResult[0].Id == item.Id);
        Assert.Contains(pagedTrades, item => tradesWithResult[1].Id == item.Id);
        Assert.Contains(pagedTrades, item => tradesWithResult[2].Id == item.Id);
        Assert.Contains(pagedTrades, item => tradesWithResult[3].Id == item.Id);
    }
    
    [Fact]
    public async Task Closed_equal_to_null_returns_all_trades_without_closed_date()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var profile = TestData.Profile.Default.Build();
        var closedTrades = Enumerable.Range(0, 2)
            .Select(_ => TestData.Trade.Default with {ProfileOrId = profile})
            .Select(x => x.Build())
            .ToList();

        var notClosedTrades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedClosed,
                Closed = openedClosed,
                ProfitLoss = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build())
            .ToList();

        DbContext.Trades.AddRange(closedTrades.Concat(notClosedTrades));
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Closed",
            Operator = "eq",
            ComparisonValue = "null",
            IsLiteral = true
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(2, pagedTrades.Count);
        Assert.Contains(pagedTrades, item => closedTrades[0].Id == item.Id);
        Assert.Contains(pagedTrades, item => closedTrades[1].Id == item.Id);
    }

    [Fact]
    public async Task Closed_not_equal_to_null_returns_all_trades_with_closed_date()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var profile = TestData.Profile.Default.Build();
        var notClosedTrades = Enumerable.Range(0, 2)
            .Select(_ => TestData.Trade.Default with {ProfileOrId = profile})
            .Select(x => x.Build())
            .ToList();

        var closedTrades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedClosed,
                Closed = openedClosed,
                ProfitLoss = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build())
            .ToList();

        DbContext.Trades.AddRange(notClosedTrades.Concat(closedTrades));
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var filter = new FilterModel
        {
            PropertyName = "Closed",
            Operator = "ne",
            ComparisonValue = "null",
            IsLiteral = true
        };

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = [filter]});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(4, pagedTrades.Count);
        Assert.Contains(pagedTrades, item => closedTrades[0].Id == item.Id);
        Assert.Contains(pagedTrades, item => closedTrades[1].Id == item.Id);
        Assert.Contains(pagedTrades, item => closedTrades[2].Id == item.Id);
        Assert.Contains(pagedTrades, item => closedTrades[3].Id == item.Id);
    }

    [Fact]
    public async Task Multiple_filters_applied_working_properly()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedClosed,
                Closed = openedClosed,
                ProfitLoss = 50m * x,
                Size = 5000m * x,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        List<FilterModel> filter =
        [
            new()
            {
                PropertyName = "Result",
                Operator = "le",
                ComparisonValue = "BreakEven",
                IsLiteral = false
            },
            new()
            {
                PropertyName = "ProfitLoss",
                Operator = "lt",
                ComparisonValue = "150",
                IsLiteral = false
            },
            new()
            {
                PropertyName = "Size",
                Operator = "gt",
                ComparisonValue = "5000",
                IsLiteral = false
            }
        ];

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profile.Id, Filter = filter});

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        var singleTrade = Assert.Single(pagedTrades);
        Assert.Equal(ResultModel.BreakEven,  singleTrade.Result);
        Assert.Equal(100m,  singleTrade.ProfitLoss);
        Assert.Equal(10_000m,  singleTrade.Size);
    }
}