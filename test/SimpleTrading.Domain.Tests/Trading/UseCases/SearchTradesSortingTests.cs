using Autofac;
using FluentAssertions;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class SearchTradesSortingTests : DomainTests
{
    private ISearchTrades Interactor => ServiceLocator.Resolve<ISearchTrades>();

    [Fact]
    public async Task Sort_by_invalid_property_does_not_work()
    {
        var sorting = new SortModel("Foobar", false);

        var response = await Interactor.Execute(new SearchTradesRequestModel {Sort = [sorting]});

        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Sort[0].Property" &&
                              x.ErrorMessage == "The sorting based on 'Foobar' does not work.");
    }

    [Fact]
    public async Task Sort_by_result_descending_works_as_intended()
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

        var sorting = new SortModel("Result", false);

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Sort = [sorting]});

        // assert
        var pagedTraded = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTraded.Which.Should().HaveCount(4);
        pagedTraded.Which.ElementAt(0).Result.Should().Be(ResultModel.Win);
        pagedTraded.Which.ElementAt(1).Result.Should().Be(ResultModel.Mediocre);
        pagedTraded.Which.ElementAt(2).Result.Should().Be(ResultModel.BreakEven);
        pagedTraded.Which.ElementAt(3).Result.Should().Be(ResultModel.Loss);
    }

    [Fact]
    public async Task If_no_sorting_was_specified_the_result_set_is_sorted_by_the_opened_date_descending()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T15:00:00").ToUtcKind();
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                Opened = initialOpenedDate.AddDays(x),
                Size = 5000m * x,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel());

        // assert
        var pagedTraded = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTraded.Which.Should().HaveCount(4);
        pagedTraded.Which.ElementAt(0).Opened.UtcDateTime.Should().Be(initialOpenedDate.AddDays(3));
        pagedTraded.Which.ElementAt(1).Opened.UtcDateTime.Should().Be(initialOpenedDate.AddDays(2));
        pagedTraded.Which.ElementAt(2).Opened.UtcDateTime.Should().Be(initialOpenedDate.AddDays(1));
        pagedTraded.Which.ElementAt(3).Opened.UtcDateTime.Should().Be(initialOpenedDate);
    }

    [Fact]
    public async Task Sort_by_closed_works_as_intended()
    {
        // arrange
        var openedClosedDateTime = DateTime.Parse("2024-08-19T18:00:00");
        var trades = Enumerable.Range(0, 2)
            .Select(x => TestData.Trade.Default with
            {
                Opened = openedClosedDateTime.AddHours(x),
                Closed = openedClosedDateTime.AddHours(x),
                Balance = 50m
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var sorting = new SortModel("Closed", false);

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Sort = [sorting]});

        // assert
        var pagedTraded = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        var firstExpected = DateTimeOffset.Parse("2024-08-19T21:00:00+02:00");
        var secondExpected = DateTimeOffset.Parse("2024-08-19T20:00:00+02:00");
        pagedTraded.Which.Should().HaveCount(2);
        pagedTraded.Which.ElementAt(0).Closed.Should().Be(firstExpected);
        pagedTraded.Which.ElementAt(1).Closed.Should().Be(secondExpected);
    }


    [Fact]
    public async Task Sort_by_does_not_trim_whitespaces()
    {
        // arrange
        var trades = Enumerable.Range(0, 2)
            .Select(x => TestData.Trade.Default with {Balance = 50m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        var sorting = new SortModel("   Balance ", false);

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel {Sort = [sorting]});

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Sort[0].Property" &&
                              x.ErrorMessage == "The sorting based on '   Balance ' does not work.");
    }
}