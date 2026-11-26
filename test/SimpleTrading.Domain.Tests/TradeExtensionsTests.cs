using SimpleTrading.TestInfrastructure.Extensions;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests;

public class TradeExtensionsTests : DomainTests
{
    [Fact]
    public void AddDefaultProfile_adds_a_profile_with_the_trade_id_and_returns_the_trade()
    {
        var trade = TestData.Trade.Default.Build();

        var result = trade.AddDefaultProfile(DbContext);

        Assert.Same(trade, result);
        Assert.Equal(trade.ProfileId, Assert.Single(DbContext.Profiles.Local, x => x.Id == trade.ProfileId).Id);
    }

    [Fact]
    public void AddDefaultAsset_adds_an_asset_with_the_trade_id_and_returns_the_trade()
    {
        var trade = TestData.Trade.Default.Build();

        var result = trade.AddDefaultAsset(DbContext);

        Assert.Same(trade, result);
        Assert.Equal(trade.AssetId, Assert.Single(DbContext.Assets.Local, x => x.Id == trade.AssetId).Id);
    }

    [Fact]
    public void AddDefaultCurrency_adds_a_currency_with_the_trade_id_and_returns_the_trade()
    {
        var trade = TestData.Trade.Default.Build();

        var result = trade.AddDefaultCurrency(DbContext);

        Assert.Same(trade, result);
        Assert.Equal(trade.CurrencyId, Assert.Single(DbContext.Currencies.Local, x => x.Id == trade.CurrencyId).Id);
    }

    [Fact]
    public void Default_references_can_be_added_in_a_chain_and_repeated_without_duplicates()
    {
        var trade = TestData.Trade.Default.Build();

        var result = trade
            .AddDefaultProfile(DbContext)
            .AddDefaultAsset(DbContext)
            .AddDefaultCurrency(DbContext)
            .AddDefaultProfile(DbContext)
            .AddDefaultAsset(DbContext)
            .AddDefaultCurrency(DbContext);

        Assert.Same(trade, result);
        Assert.Single(DbContext.Profiles.Local, x => x.Id == trade.ProfileId);
        Assert.Single(DbContext.Assets.Local, x => x.Id == trade.AssetId);
        Assert.Single(DbContext.Currencies.Local, x => x.Id == trade.CurrencyId);
    }

    [Fact]
    public void Existing_tracked_references_are_not_added_again()
    {
        var trade = TestData.Trade.Default.Build();
        var profile = (TestData.Profile.Default with {Id = trade.ProfileId}).Build();
        var asset = (TestData.Asset.Default with {Id = trade.AssetId}).Build();
        var currency = (TestData.Currency.Default with {Id = trade.CurrencyId}).Build();
        DbContext.AddRange(profile, asset, currency);

        trade
            .AddDefaultProfile(DbContext)
            .AddDefaultAsset(DbContext)
            .AddDefaultCurrency(DbContext);

        Assert.Same(profile, DbContext.Profiles.Find(trade.ProfileId));
        Assert.Same(asset, DbContext.Assets.Find(trade.AssetId));
        Assert.Same(currency, DbContext.Currencies.Find(trade.CurrencyId));
        Assert.Single(DbContext.Profiles.Local, x => x.Id == trade.ProfileId);
        Assert.Single(DbContext.Assets.Local, x => x.Id == trade.AssetId);
        Assert.Single(DbContext.Currencies.Local, x => x.Id == trade.CurrencyId);
    }

}