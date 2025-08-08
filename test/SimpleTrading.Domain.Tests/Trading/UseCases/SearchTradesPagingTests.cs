using Autofac;
using FluentValidation.Results;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;
using SimpleTrading.Domain.Trading.UseCases.Shared;
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
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
            Page = 1,
            PageSize = 5
        });

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(5, pagedTrades.Count);
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
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
            Page = 1,
            PageSize = 5
        });

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(20, pagedTrades.TotalCount);
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
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
            Page = 1,
            PageSize = 5
        });

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(4, pagedTrades.TotalPages);
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
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
            Page = 2,
            PageSize = 5
        });

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.False(pagedTrades.IsFirstPage);
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
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
            Page = 1,
            PageSize = 5
        });

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.True(pagedTrades.IsFirstPage);
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
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
            Page = 4,
            PageSize = 5
        });

        // assert
        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        Assert.Equal(3, pagedTrades.Count);
        Assert.True(pagedTrades.IsLastPage);
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

        var validationResult = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("PageSize", error.PropertyName);
        Assert.Equal("'Page Size' must be greater than or equal to '1'.", error.ErrorMessage);
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

        var validationResult = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("Page", error.PropertyName);
        Assert.Equal("'Page' must be greater than or equal to '1'.", error.ErrorMessage);
    }
}