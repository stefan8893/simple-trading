using Autofac;
using AwesomeAssertions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class SearchTradesProfileTests : DomainTests
{
    private ISearchTrades Interactor => ServiceLocator.Resolve<ISearchTrades>();
    
    
    [Fact]
    public async Task Empty_profile_id_returns_no_trades()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        var profile = TestData.Profile.Default.Build();
        var trades = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profile, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Profiles.Add(profile);
        DbContext.Trades.AddRange(trades);
        await DbContext.SaveChangesAsync();

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = Guid.Empty});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().BeEmpty();
    }
    
    [Fact]
    public async Task Searching_for_trades_depends_always_on_the_profile_to_which_the_trades_belong()
    {
        // arrange
        var initialOpenedDate = DateTime.Parse("2024-08-19T14:00:00");
        
        var profileA = TestData.Profile.Default.Build();
        var tradesA = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profileA, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build())
            .ToList();
        
        var profileB = TestData.Profile.Default.Build();
        var tradesB = Enumerable.Range(0, 3)
            .Select(x => TestData.Trade.Default with {ProfileOrId = profileB, Opened = initialOpenedDate.AddHours(x)})
            .Select(x => x.Build());

        DbContext.Profiles.AddRange(profileA, profileB);
        DbContext.Trades.AddRange(tradesA);
        DbContext.Trades.AddRange(tradesB);
        await DbContext.SaveChangesAsync();

        // act
        var response =
            await Interactor.Execute(new SearchTradesRequestModel {ProfileId = profileA.Id});

        // assert
        var pagedTrades = response.Value.Should().BeOfType<PagedList<TradeResponseModel>>();
        pagedTrades.Which.Should().HaveCount(3)
            .And.Contain(x => tradesA.Any(y => y.Id == x.Id));
    }
}