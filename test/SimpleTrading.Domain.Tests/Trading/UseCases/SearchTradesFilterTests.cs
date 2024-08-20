using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class SearchTradesTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private ISearchTrades Interactor => ServiceLocator.GetRequiredService<ISearchTrades>();

    [Fact]
    public async Task Greater_than_opened_date_with_comparison_value_in_local_time_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "gt",
            ComparisonValue = "2024-08-19T17:00:00+02:00",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var expected = DateTimeOffset.Parse("2024-08-19T18:00:00+02:00");
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(1)
            .And.Contain(x => x.Opened == expected);
    }

    [Fact]
    public async Task Greater_than_opened_date_with_comparison_value_in_utc_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "gt",
            ComparisonValue = "2024-08-19T15:00:00Z",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var expected = DateTimeOffset.Parse("2024-08-19T18:00:00+02:00");
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(1)
            .And.Contain(x => x.Opened == expected);
    }

    [Fact]
    public async Task Closed_greater_than_null_returns_bad_input()
    {
        // arrange
        var filter = new FilterModel
        {
            PropertyName = "Closed",
            Operator = "gt",
            ComparisonValue = "null",
            IsLiteral = true
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "Null is not allowed here.");
    }

    [Fact]
    public async Task Greater_than_opened_date_with_invalid_comparison_value_returns_bad_input()
    {
        // arrange
        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "gt",
            ComparisonValue = "2024-08-19T17:00:",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Filter[0].ComparisonValue" &&
                              x.ErrorMessage == "The value '2024-08-19T17:00:' is not valid.");
    }

    [Fact]
    public async Task Greater_than_opened_date_with_typo_in_operator_returns_bad_input()
    {
        // arrange
        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "grt",
            ComparisonValue = "2024-08-19T17:00:00",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Filter[0].Operator" &&
                              x.ErrorMessage == "The operator 'grt' is not supported.");
    }


    [Fact]
    public async Task Greater_than_opened_date_with_typo_in_property_name_returns_bad_input()
    {
        // arrange
        var filter = new FilterModel
        {
            PropertyName = "Openend",
            Operator = "gt",
            ComparisonValue = "2024-08-19T17:00:00",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Filter[0].PropertyName" &&
                              x.ErrorMessage == "'Openend' cannot be used as a filter.");
    }

    [Fact]
    public async Task Greater_than_or_equal_to_opened_date_with_comparison_value_in_local_time_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "ge",
            ComparisonValue = "2024-08-19T17:00:00+02:00",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var firstExpected = DateTimeOffset.Parse("2024-08-19T17:00:00+02:00");
        var secondExpected = DateTimeOffset.Parse("2024-08-19T18:00:00+02:00");
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(2)
            .And.Contain(x => x.Opened == firstExpected)
            .And.Contain(x => x.Opened == secondExpected);
    }

    [Fact]
    public async Task Less_than_or_equal_to_opened_date_with_comparison_value_in_local_time_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "le",
            ComparisonValue = "2024-08-19T17:00:00+02:00",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var firstExpected = DateTimeOffset.Parse("2024-08-19T16:00:00+02:00");
        var secondExpected = DateTimeOffset.Parse("2024-08-19T17:00:00+02:00");
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(2)
            .And.Contain(x => x.Opened == firstExpected)
            .And.Contain(x => x.Opened == secondExpected);
    }

    [Fact]
    public async Task Less_than_opened_date_with_comparison_value_in_local_time_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "lt",
            ComparisonValue = "2024-08-19T17:00:00+02:00",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var expected = DateTimeOffset.Parse("2024-08-19T16:00:00+02:00");
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(1)
            .And.Contain(x => x.Opened == expected);
    }

    [Fact]
    public async Task Equal_to_opened_date_with_comparison_value_in_local_time_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "eq",
            ComparisonValue = "2024-08-19T17:00:00+02:00",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var expected = DateTimeOffset.Parse("2024-08-19T17:00:00+02:00");
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(1)
            .And.Contain(x => x.Opened == expected);
    }

    [Fact]
    public async Task Not_equal_to_opened_date_with_comparison_value_in_local_time_returns_correct_result()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Opened",
            Operator = "ne",
            ComparisonValue = "2024-08-19T17:00:00+02:00",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var firstExpected = DateTimeOffset.Parse("2024-08-19T16:00:00+02:00");
        var secondExpected = DateTimeOffset.Parse("2024-08-19T18:00:00+02:00");
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(2)
            .And.Contain(x => x.Opened == firstExpected)
            .And.Contain(x => x.Opened == secondExpected);
    }

    [Fact]
    public async Task Greater_than_balance_with_uppercase_property_name_returns_correct_result()
    {
        // arrange
        var trades = Enumerable.Range(1, 2)
            .Select(x => TestData.Trade.Default with {Balance = 500m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "BALANCE",
            Operator = "gt",
            ComparisonValue = "500",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(1)
            .And.Contain(x => x.Balance == 1000m);
    }

    [Fact]
    public async Task Greater_than_balance_with_empty_comparison_value_returns_bad_input()
    {
        // arrange
        var filter = new FilterModel
        {
            PropertyName = "Balance",
            Operator = "gt",
            ComparisonValue = "",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Filter[0].ComparisonValue" &&
                              x.ErrorMessage == "'Comparison value' must not be empty.");
    }

    [Fact]
    public async Task Greater_than_balance_with_empty_operator_returns_bad_input()
    {
        // arrange
        var filter = new FilterModel
        {
            PropertyName = "Balance",
            Operator = "",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Filter[0].Operator" &&
                              x.ErrorMessage == "'Operator' must not be empty.");
    }

    [Fact]
    public async Task Greater_than_balance_with_empty_property_name_returns_bad_input()
    {
        // arrange
        var filter = new FilterModel
        {
            PropertyName = "",
            Operator = "gt",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Filter[0].PropertyName" &&
                              x.ErrorMessage == "'Field' must not be empty.");
    }

    [Fact]
    public async Task Unknown_property_names_cannot_be_used_as_a_filter()
    {
        // arrange
        var filter = new FilterModel
        {
            PropertyName = "Foobar",
            Operator = "gt",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Filter[0].PropertyName" &&
                              x.ErrorMessage == "'Foobar' cannot be used as a filter.");
    }

    [Fact]
    public async Task An_unknown_operator_cannot_be_used_in_a_filter()
    {
        // arrange
        var filter = new FilterModel
        {
            PropertyName = "Balance",
            Operator = "gr",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Filter[0].Operator" &&
                              x.ErrorMessage == "The operator 'gr' is not supported.");
    }

    [Fact]
    public async Task A_null_property_name_returns_bad_input()
    {
        // arrange
        var filter = new FilterModel
        {
            PropertyName = null!,
            Operator = "gt",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Filter[0].PropertyName" &&
                              x.ErrorMessage == "'Field' must not be empty.");
    }

    [Fact]
    public async Task Property_names_with_whitespaces_will_not_be_trimmed_and_bad_input_gets_returned()
    {
        // arrange
        var filter = new FilterModel
        {
            PropertyName = "balance ",
            Operator = "gt",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Filter[0].PropertyName" &&
                              x.ErrorMessage == "'balance ' cannot be used as a filter.");
    }

    [Fact]
    public async Task Operators_with_whitespaces_will_not_be_trimmed_and_bad_input_gets_returned()
    {
        // arrange
        var filter = new FilterModel
        {
            PropertyName = "balance",
            Operator = " gt",
            ComparisonValue = "50",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Filter[0].Operator" &&
                              x.ErrorMessage == "The operator ' gt' is not supported.");
    }

    [Fact]
    public async Task Greater_than_or_equal_to_size_with_valid_input_returns_correct_result()
    {
        // arrange
        var trades = Enumerable.Range(1, 2)
            .Select(x => TestData.Trade.Default with {Size = 5000m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Size",
            Operator = "ge",
            ComparisonValue = "10000",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(1)
            .And.Contain(x => x.Size == 10_000m);
    }

    [Fact]
    public async Task Less_than_or_equal_to_size_with_valid_input_returns_correct_result()
    {
        // arrange
        var trades = Enumerable.Range(1, 2)
            .Select(x => TestData.Trade.Default with {Size = 5000m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Size",
            Operator = "le",
            ComparisonValue = "5000",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(1)
            .And.Contain(x => x.Size == 5000m);
    }

    [Fact]
    public async Task Not_equal_to_size_with_valid_input_returns_correct_result()
    {
        // arrange
        var trades = Enumerable.Range(1, 2)
            .Select(x => TestData.Trade.Default with {Size = 5000m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Size",
            Operator = "ne",
            ComparisonValue = "5000",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(1)
            .And.Contain(x => x.Size == 10_000m);
    }

    [Fact]
    public async Task Less_than_balance_with_valid_input_returns_correct_result()
    {
        // arrange
        var trades = Enumerable.Range(1, 2)
            .Select(x => TestData.Trade.Default with {Balance = 500m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Balance",
            Operator = "lt",
            ComparisonValue = "500",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().BeEmpty();
    }

    [Fact]
    public async Task Equal_to_balance_with_valid_input_returns_correct_result()
    {
        // arrange
        var trades = Enumerable.Range(1, 2)
            .Select(x => TestData.Trade.Default with {Balance = 500m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Balance",
            Operator = "eq",
            ComparisonValue = "500",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(1)
            .And.Contain(x => x.Balance == 500m);
    }

    [Fact]
    public async Task Greater_than_result_with_invalid_comparison_value_returns_bad_input()
    {
        // arrange
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with {Result = (ResultModel) x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "gt",
            ComparisonValue = "NotThatBad",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Filter[0].ComparisonValue" &&
                              x.ErrorMessage == "The value 'NotThatBad' is not valid.");
    }

    [Fact]
    public async Task Greater_than_result_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosed,
                Closed = openedClosed,
                Balance = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "gt",
            ComparisonValue = "BreakEven",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(2)
            .And.Contain(x => x.Result == ResultModel.Mediocre)
            .And.Contain(x => x.Result == ResultModel.Win);
    }


    [Fact]
    public async Task Greater_than_closed_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosed.AddHours(x),
                Closed = openedClosed.AddHours(x),
                Balance = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Closed",
            Operator = "gt",
            ComparisonValue = "2024-08-19T15:00:00Z",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(3)
            .And.NotContain(x => x.Closed!.Value.UtcDateTime == openedClosed);
    }

    [Fact]
    public async Task Greater_than_or_equal_to_result_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosed,
                Closed = openedClosed,
                Balance = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "ge",
            ComparisonValue = "BreakEven",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(3)
            .And.Contain(x => x.Result == ResultModel.BreakEven)
            .And.Contain(x => x.Result == ResultModel.Mediocre)
            .And.Contain(x => x.Result == ResultModel.Win);
    }

    [Fact]
    public async Task Less_than_or_equal_to_result_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosed,
                Closed = openedClosed,
                Balance = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "le",
            ComparisonValue = "BreakEven",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(2)
            .And.Contain(x => x.Result == ResultModel.BreakEven)
            .And.Contain(x => x.Result == ResultModel.Loss);
    }

    [Fact]
    public async Task Less_than_to_result_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosed,
                Closed = openedClosed,
                Balance = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "lt",
            ComparisonValue = "Mediocre",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(2)
            .And.Contain(x => x.Result == ResultModel.BreakEven)
            .And.Contain(x => x.Result == ResultModel.Loss);
    }

    [Fact]
    public async Task Equal_to_result_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosed,
                Closed = openedClosed,
                Balance = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "eq",
            ComparisonValue = "Mediocre",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(1)
            .And.Contain(x => x.Result == ResultModel.Mediocre);
    }

    [Fact]
    public async Task Not_equal_to_result_with_valid_input_returns_correct_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosed,
                Closed = openedClosed,
                Balance = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "ne",
            ComparisonValue = "Mediocre",
            IsLiteral = false
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(3)
            .And.Contain(x => x.Result == ResultModel.Loss)
            .And.Contain(x => x.Result == ResultModel.BreakEven)
            .And.Contain(x => x.Result == ResultModel.Win);
    }

    [Fact]
    public async Task Equal_to_null_result_returns_all_trades_without_a_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var tradesWithoutResult = Enumerable.Range(0, 2)
            .Select(x => TestData.Trade.Default.Build())
            .ToList();

        var tradesWithResult = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosed,
                Closed = openedClosed,
                Balance = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build())
            .ToList();

        DbContext.Trades.AddRange(tradesWithoutResult.Concat(tradesWithResult));
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "eq",
            ComparisonValue = "null",
            IsLiteral = true
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(2)
            .And.Contain(x => x.Id == tradesWithoutResult[0].Id)
            .And.Contain(x => x.Id == tradesWithoutResult[1].Id);
    }

    [Fact]
    public async Task Not_equal_to_null_result_returns_all_trades_with_result()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var tradesWithoutResult = Enumerable.Range(0, 2)
            .Select(x => TestData.Trade.Default.Build())
            .ToList();

        var tradesWithResult = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosed,
                Closed = openedClosed,
                Balance = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build())
            .ToList();

        DbContext.Trades.AddRange(tradesWithoutResult.Concat(tradesWithResult));
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Result",
            Operator = "ne",
            ComparisonValue = "null",
            IsLiteral = true
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(4)
            .And.Contain(x => x.Id == tradesWithResult[0].Id)
            .And.Contain(x => x.Id == tradesWithResult[1].Id)
            .And.Contain(x => x.Id == tradesWithResult[2].Id)
            .And.Contain(x => x.Id == tradesWithResult[3].Id);
    }

    [Fact]
    public async Task Closed_equal_to_null_returns_all_trades_without_closed_date()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var closedTrades = Enumerable.Range(0, 2)
            .Select(x => TestData.Trade.Default.Build())
            .ToList();

        var notClosedTrades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosed,
                Closed = openedClosed,
                Balance = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build())
            .ToList();

        DbContext.Trades.AddRange(closedTrades.Concat(notClosedTrades));
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Closed",
            Operator = "eq",
            ComparisonValue = "null",
            IsLiteral = true
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(2)
            .And.Contain(x => x.Id == closedTrades[0].Id)
            .And.Contain(x => x.Id == closedTrades[1].Id);
    }

    [Fact]
    public async Task Closed_not_equal_to_null_returns_all_trades_with_closed_date()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var notClosedTrades = Enumerable.Range(0, 2)
            .Select(x => TestData.Trade.Default.Build())
            .ToList();

        var closedTrades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosed,
                Closed = openedClosed,
                Balance = 50m,
                Result = (ResultModel) x
            })
            .Select(x => x.Build())
            .ToList();

        DbContext.Trades.AddRange(notClosedTrades.Concat(closedTrades));
        await DbContext.SaveChangesAsync();

        var filter = new FilterModel
        {
            PropertyName = "Closed",
            Operator = "ne",
            ComparisonValue = "null",
            IsLiteral = true
        };

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = [filter]});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(4)
            .And.Contain(x => x.Id == closedTrades[0].Id)
            .And.Contain(x => x.Id == closedTrades[1].Id)
            .And.Contain(x => x.Id == closedTrades[2].Id)
            .And.Contain(x => x.Id == closedTrades[3].Id);
    }

    [Fact]
    public async Task Multiple_filters_applied_working_properly()
    {
        // arrange
        var openedClosed = DateTime.Parse("2024-08-19T15:00:00");
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosed,
                Closed = openedClosed,
                Balance = 50m * x,
                Size = 5000m * x,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        List<FilterModel> filter =
        [
            new FilterModel
            {
                PropertyName = "Result",
                Operator = "le",
                ComparisonValue = "BreakEven",
                IsLiteral = false
            },
            new FilterModel
            {
                PropertyName = "Balance",
                Operator = "lt",
                ComparisonValue = "150",
                IsLiteral = false
            },
            new FilterModel
            {
                PropertyName = "Size",
                Operator = "gt",
                ComparisonValue = "5000",
                IsLiteral = false
            }
        ];

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Filter = filter});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(1)
            .And.Contain(x => x.Result == ResultModel.BreakEven)
            .And.Contain(x => x.Balance == 100m)
            .And.Contain(x => x.Size == 10_000m);
    }
}