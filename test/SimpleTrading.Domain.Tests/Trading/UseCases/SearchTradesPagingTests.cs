using Autofac;
using AwesomeAssertions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class SearchTradesPagingTests : DomainTests
{
    private ISearchTrades Interactor => ServiceLocator.Resolve<ISearchTrades>();

    [Fact]
    public async Task Paged_result_contains_only_requested_subset()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T02:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 20)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
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
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 20)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
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
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 20)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
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
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 20)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
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
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 20)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
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
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 18)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
            Page = 4, PageSize = 5
        });

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(3);
        pagedTrades.Which.IsLastPage.Should().BeTrue();
    }

    [Fact]
    public async Task Zero_is_not_a_valid_page_size()
    {
        var profile = TestData.Profile.Default.Build();
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
            PageSize = 0
        });

        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "PageSize" &&
                              x.ErrorMessage == "'Page size' must be greater than or equal to '1'.");
    }

    [Fact]
    public async Task Zero_is_not_a_valid_page_they_start_at_one()
    {
        var profile = TestData.Profile.Default.Build();
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
            Page = 0
        });

        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Page" &&
                              x.ErrorMessage == "'Page' must be greater than or equal to '1'.");
    }
}