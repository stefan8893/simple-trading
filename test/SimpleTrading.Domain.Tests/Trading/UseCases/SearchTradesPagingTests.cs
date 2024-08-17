using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class SearchTradesPagingTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private ISearchTrades Interactor => ServiceLocator.GetRequiredService<ISearchTrades>();

    [Fact]
    public async Task Paged_result_contains_only_requested_subset()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T02:00:00");
        var trades = Enumerable.Range(0, 20)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            Page = 1, PageSize = 5
        });

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(5);
        pagedTrades.Which.Count.Should().Be(5);
    }

    [Fact]
    public async Task Paged_result_has_correct_total_count()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T02:00:00");
        var trades = Enumerable.Range(0, 20)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            Page = 1, PageSize = 5
        });

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.TotalCount.Should().Be(20);
    }

    [Fact]
    public async Task Paged_result_has_correct_total_pages_count()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T02:00:00");
        var trades = Enumerable.Range(0, 20)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            Page = 1, PageSize = 5
        });

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.TotalPages.Should().Be(4);
    }

    [Fact]
    public async Task IsFirstPage_is_false_on_second_page()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T02:00:00");
        var trades = Enumerable.Range(0, 20)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            Page = 2, PageSize = 5
        });

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.IsFirstPage.Should().BeFalse();
    }

    [Fact]
    public async Task IsFirstPage_is_true_on_first_page()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T02:00:00");
        var trades = Enumerable.Range(0, 20)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            Page = 1, PageSize = 5
        });

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.IsFirstPage.Should().BeTrue();
    }

    [Fact]
    public async Task Last_page_is_not_full_if_total_count_is_not_pageSize_times_pages()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T02:00:00");
        var trades = Enumerable.Range(0, 18)
            .Select(x => TestData.Trade.Default with {Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            Page = 4, PageSize = 5
        });

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(3);
        pagedTrades.Which.IsLastPage.Should().BeTrue();
    }
}