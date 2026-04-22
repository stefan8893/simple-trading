using Autofac;
using FluentValidation.Results;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class SearchTradesSortingTests : DomainTests
{
    private ISearchTrades Interactor => ServiceLocator.Resolve<ISearchTrades>();

    [Fact]
    public async Task Sort_by_invalid_property_does_not_work()
    {
        var sorting = new SortModel("Foobar", false);

        var profile = TestData.Profile.Default.Build();
        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
            Sort = [sorting]
        }, TestContext.Current.CancellationToken);

        var validationResult = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("The sorting based on 'Foobar' does not work.", error.ErrorMessage);
        Assert.Equal("Sort[0].Property", error.PropertyName);
    }

    [Fact]
    public async Task Sort_by_result_descending_works_as_intended()
    {
        var openedFinished = DateTime.Parse("2024-08-19T15:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedFinished,
                Finished = openedFinished,
                ProfitLoss = 50m * x,
                Size = 5000m * x,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sorting = new SortModel("Result", false);

        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
            Sort = [sorting]
        }, TestContext.Current.CancellationToken);

        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);

        Assert.Collection(pagedTrades,
            first => Assert.Equal(ResultModel.Win, first.Result),
            second => Assert.Equal(ResultModel.Mediocre, second.Result),
            third => Assert.Equal(ResultModel.BreakEven, third.Result),
            fourth => Assert.Equal(ResultModel.Loss, fourth.Result)
        );
    }

    [Fact]
    public async Task If_no_sorting_was_specified_the_result_set_is_sorted_by_the_opened_date_descending()
    {
        var initialOpenedDate = DateTime.Parse("2024-08-19T15:00:00").ToUtcKind();
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 4)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = initialOpenedDate.AddDays(x),
                Size = 5000m * x,
                Result = (ResultModel) x
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id
        }, TestContext.Current.CancellationToken);

        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);

        Assert.Collection(pagedTrades,
            first => Assert.Equal(initialOpenedDate.AddDays(3), first.Opened.UtcDateTime),
            second => Assert.Equal(initialOpenedDate.AddDays(2), second.Opened.UtcDateTime),
            third => Assert.Equal(initialOpenedDate.AddDays(1), third.Opened.UtcDateTime),
            fourth => Assert.Equal(initialOpenedDate, fourth.Opened.UtcDateTime)
        );
    }

    [Fact]
    public async Task Sort_by_finished_works_as_intended()
    {
        var openedFinishedDateTime = DateTime.Parse("2024-08-19T18:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 2)
            .Select(x => TestData.Trade.Default with
            {
                ProfileOrId = profile,
                Opened = openedFinishedDateTime.AddHours(x),
                Finished = openedFinishedDateTime.AddHours(x),
                ProfitLoss = 50m
            })
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sorting = new SortModel("Finished", false);

        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
            Sort = [sorting]
        }, TestContext.Current.CancellationToken);

        var pagedTrades = Assert.IsType<PagedList<TradeResponseModel>>(response.Value);
        var firstExpected = DateTimeOffset.Parse("2024-08-19T21:00:00+02:00");
        var secondExpected = DateTimeOffset.Parse("2024-08-19T20:00:00+02:00");

        Assert.Collection(pagedTrades,
            first => Assert.Equal(firstExpected, first.Finished),
            second => Assert.Equal(secondExpected, second.Finished));
    }


    [Fact]
    public async Task Sort_by_does_not_trim_whitespaces()
    {
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 2)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, ProfitLoss = 50m * x})
            .Select(x => x.Build());

        DbContext.Trades.AddRange(trades);
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sorting = new SortModel("   ProfitLoss ", false);

        var response = await Interactor.Execute(new SearchTradesRequestModel
        {
            ProfileId = profile.Id,
            Sort = [sorting]
        }, TestContext.Current.CancellationToken);

        var validationResult = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("The sorting based on '   ProfitLoss ' does not work.", error.ErrorMessage);
        Assert.Equal("Sort[0].Property", error.PropertyName);
    }
}