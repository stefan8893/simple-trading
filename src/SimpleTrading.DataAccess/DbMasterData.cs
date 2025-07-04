﻿using SimpleTrading.Domain;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.User;

namespace SimpleTrading.DataAccess;

public class DbMasterData(TradingDbContext dbContext)
{
    private static readonly DateTime InitialCreationDateTime = DateTime.Parse("2024-08-03T08:00:00").ToUtcKind();

    public async Task Populate()
    {
        var profiles = CreateProfiles();
        dbContext.Profiles.AddRange(profiles);

        var assets = CreateAssets();
        dbContext.Assets.AddRange(assets);

        var currencies = CreateCurrencies();
        dbContext.Currencies.AddRange(currencies);

        await PopulateUserSettings();

        await dbContext.SaveChangesAsync();
    }

    public async Task PopulateUserSettings()
    {
        var userSettings = new UserSettings
        {
            Id = Constants.UserSettingsId,
            Culture = Constants.DefaultCulture.Name,
            Language = null,
            TimeZone = Constants.DefaultTimeZone,
            LastModified = InitialCreationDateTime,
            Created = InitialCreationDateTime
        };

        dbContext.UserSettings.Add(userSettings);
        await dbContext.SaveChangesAsync();
    }

    private static IReadOnlyList<Profile> CreateProfiles()
    {
        var defaultProfile = new Profile
        {
            Id = Guid.Parse("401c519b-956a-4a5f-bd84-77e716817771"),
            Name = "Default",
            Description = null,
            IsActive = true,
            Created = DateTime.Parse("2024-08-03T08:00:00Z")
        };

        return [defaultProfile];
    }

    private static IReadOnlyList<Currency> CreateCurrencies()
    {
        var eur = new Currency
        {
            Id = Guid.Parse("dd1f1281-7ec9-450e-8dd8-da1f4eb78629"),
            IsoCode = "EUR",
            Name = "Euro",
            Created = InitialCreationDateTime
        };

        var usd = new Currency
        {
            Id = Guid.Parse("50318871-fb0b-457f-8e7d-9e9bf2270acc"),
            IsoCode = "USD",
            Name = "US-Dollar",
            Created = InitialCreationDateTime
        };

        var btc = new Currency
        {
            Id = Guid.Parse("82d14e77-2812-4e00-b54d-56e1d3937080"),
            IsoCode = "BTC",
            Name = "Bitcoin",
            Created = InitialCreationDateTime
        };

        return [eur, usd, btc];
    }

    private static IReadOnlyList<Asset> CreateAssets()
    {
        var eurUsd = new Asset
        {
            Id = Guid.Parse("0c275c78-0508-4836-81d5-342e2445d60c"),
            Symbol = "EURUSD",
            Name = "EUR/USD",
            Created = InitialCreationDateTime
        };

        var btcEur = new Asset
        {
            Id = Guid.Parse("119db4a7-3661-47d5-8a2c-c5a4aeb5b366"),
            Symbol = "BTCEUR",
            Name = "BTC/EUR",
            Created = InitialCreationDateTime
        };

        var btcUsd = new Asset
        {
            Id = Guid.Parse("f324c4e0-6342-4d77-9f5e-72e045471326"),
            Symbol = "BTCUSD",
            Name = "BTC/USD",
            Created = InitialCreationDateTime
        };

        var sp500 = new Asset
        {
            Id = Guid.Parse("05426495-9caf-4d0f-a7de-5297a35c2af6"),
            Symbol = "S500",
            Name = "S&P 500",
            Created = InitialCreationDateTime
        };

        return [eurUsd, btcEur, btcUsd, sp500];
    }
}