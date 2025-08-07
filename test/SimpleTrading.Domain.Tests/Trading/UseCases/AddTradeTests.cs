using System.Globalization;
using Autofac;
using FluentValidation.Results;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.AddTrade;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class AddTradeTests : DomainTests
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-05T14:00:00").ToUtcKind();

    private IAddTrade Interactor => ServiceLocator.Resolve<IAddTrade>();

    protected override void OverrideServices(ContainerBuilder builder)
    {
        builder.Register<UtcNow>(_ => () => _utcNow);
    }

    [Fact]
    public async Task Asset_id_must_not_be_empty()
    {
        var requestModel = new AddTradeRequestModel
        {
            AssetId = Guid.Empty,
            ProfileId = TestData.Profile.Default.Build().Id,
            Opened = new DateTimeOffset(_utcNow),
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = TestData.Currency.Default.Build().Id
        };

        var response = await Interactor.Execute(requestModel);

        var badInput = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(badInput.Errors);
        Assert.Equal("'Asset' must not be empty.", error.ErrorMessage);
        Assert.Equal("AssetId", error.PropertyName);
    }

    [Fact]
    public async Task Profile_id_must_not_be_empty()
    {
        var requestModel = new AddTradeRequestModel
        {
            AssetId = TestData.Asset.Default.Build().Id,
            ProfileId = Guid.Empty,
            Opened = new DateTimeOffset(_utcNow),
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = TestData.Currency.Default.Build().Id
        };

        var response = await Interactor.Execute(requestModel);

        var badInput = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(badInput.Errors);
        Assert.Equal("'Profile' must not be empty.", error.ErrorMessage);
        Assert.Equal("ProfileId", error.PropertyName);
    }

    [Fact]
    public async Task Currency_id_must_not_be_empty()
    {
        var requestModel = new AddTradeRequestModel
        {
            AssetId = TestData.Asset.Default.Build().Id,
            ProfileId = TestData.Profile.Default.Build().Id,
            Opened = new DateTimeOffset(_utcNow),
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = Guid.Empty
        };

        var response = await Interactor.Execute(requestModel);

        var badInput = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(badInput.Errors);
        Assert.Equal("'Currency' must not be empty.", error.ErrorMessage);
        Assert.Equal("CurrencyId", error.PropertyName);
    }

    [Theory]
    [InlineData("en-US", "'Trade Size' must be greater than '0'.")]
    [InlineData("de-AT", "Der Wert von 'Handelsvolumen' muss grösser sein als '0'.")]
    public async Task Size_must_be_above_zero(string culture, string errorMessage)
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

        var requestModel = new AddTradeRequestModel
        {
            AssetId = TestData.Asset.Default.Build().Id,
            ProfileId = TestData.Profile.Default.Build().Id,
            Opened = new DateTimeOffset(_utcNow),
            Size = 0,
            EntryPrice = 1.05m,
            CurrencyId = TestData.Currency.Default.Build().Id
        };

        var response = await Interactor.Execute(requestModel);

        var badInput = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(badInput.Errors);
        Assert.Equal(errorMessage, error.ErrorMessage);
        Assert.Equal("Size", error.PropertyName);
    }

    [Theory]
    [InlineData("en-US", "'Result' has a range of values which does not include '50'.")]
    [InlineData("de-AT", "'Ergebnis' hat einen Wertebereich, der '50' nicht enthält.")]
    public async Task Result_must_be_in_enum_range(string culture, string errorMessage)
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

        var requestModel = new AddTradeRequestModel
        {
            AssetId = TestData.Asset.Default.Build().Id,
            ProfileId = TestData.Profile.Default.Build().Id,
            Opened = new DateTimeOffset(_utcNow),
            Closed = new DateTimeOffset(_utcNow),
            ProfitLoss = 500,
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = TestData.Currency.Default.Build().Id,
            ManuallyEnteredResult = (ResultModel)50
        };

        var response = await Interactor.Execute(requestModel);

        var badInput = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(badInput.Errors);
        Assert.Equal(errorMessage, error.ErrorMessage);
        Assert.Equal("ManuallyEnteredResult", error.PropertyName);
    }

    [Theory]
    [InlineData("de-AT", "Der Wert von 'Eröffnet' muss grösser oder gleich '01.01.2000 00:00:00' sein.")]
    [InlineData("en-US", "'Opened' must be greater than or equal to '01.01.2000 00:00:00'.")]
    public async Task Opened_must_not_be_before_min_date(string culture, string errorMessage)
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

        var longTimeAgo = new DateTimeOffset(DateTime.Parse("1998-08-05T12:00:00").ToUtcKind());

        var requestModel = new AddTradeRequestModel
        {
            AssetId = TestData.Asset.Default.Build().Id,
            ProfileId = TestData.Profile.Default.Build().Id,
            Opened = longTimeAgo,
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = TestData.Currency.Default.Build().Id
        };

        var response = await Interactor.Execute(requestModel);

        var badInput = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(badInput.Errors);
        Assert.Equal(errorMessage, error.ErrorMessage);
        Assert.Equal("Opened", error.PropertyName);
    }

    [Fact]
    public async Task Opened_must_not_be_greater_than_one_day_in_the_future()
    {
        var opened = new DateTimeOffset(_utcNow.AddDays(1).AddSeconds(1));

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();

        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = opened,
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id
        };

        var response = await Interactor.Execute(requestModel);

        var badInput = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(badInput.Errors);
        Assert.Equal("'Opened' must be less than or equal to '06.08.2024 16:00'.", error.ErrorMessage);
        Assert.Equal("Opened", error.PropertyName);
    }

    [Fact]
    public async Task Reference_link_must_be_a_valid_uri()
    {
        var reference = new ReferenceRequestModel(ReferenceType.Other, "foobar");

        var requestModel = new AddTradeRequestModel
        {
            AssetId = TestData.Asset.Default.Build().Id,
            ProfileId = TestData.Profile.Default.Build().Id,
            Opened = new DateTimeOffset(_utcNow),
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = TestData.Currency.Default.Build().Id,
            References = [reference]
        };

        var response = await Interactor.Execute(requestModel);

        var badInput = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(badInput.Errors);
        Assert.Equal("Invalid link.", error.ErrorMessage);
        Assert.Equal("References[0].Link", error.PropertyName);
    }

    [Theory]
    [InlineData("en-US", "The length of 'Notes' must be 4000 characters or fewer. You entered 40001 characters.")]
    [InlineData("de-AT",
        "Die Länge von 'Anmerkungen' muss kleiner oder gleich 4000 sein. Sie haben 40001 Zeichen eingegeben.")]
    public async Task Reference_notes_with_more_than_4000_chars_are_not_allowed(string culture, string errorMessage)
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

        var reference = new ReferenceRequestModel(ReferenceType.Other,
            "https://example.org",
            new string('a', 40001));

        var requestModel = new AddTradeRequestModel
        {
            AssetId = TestData.Asset.Default.Build().Id,
            ProfileId = TestData.Profile.Default.Build().Id,
            Opened = new DateTimeOffset(_utcNow),
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = TestData.Currency.Default.Build().Id,
            References = [reference]
        };

        var response = await Interactor.Execute(requestModel);

        var badInput = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(badInput.Errors);
        Assert.Equal(errorMessage, error.ErrorMessage);
        Assert.Equal("References[0].Notes", error.PropertyName);
    }

    [Fact]
    public async Task Notes_with_more_than_4000_chars_are_not_allowed()
    {
        var requestModel = new AddTradeRequestModel
        {
            AssetId = TestData.Asset.Default.Build().Id,
            ProfileId = TestData.Profile.Default.Build().Id,
            Opened = new DateTimeOffset(_utcNow),
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = TestData.Currency.Default.Build().Id,
            Notes = new string('a', 4001)
        };

        var response = await Interactor.Execute(requestModel);

        var badInput = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(badInput.Errors);
        Assert.Equal("The length of 'Notes' must be 4000 characters or fewer. You entered 4001 characters.", 
            error.ErrorMessage);
        Assert.Equal("Notes", error.PropertyName);
    }

    [Fact]
    public async Task You_cant_add_a_trade_if_the_currency_does_not_exist()
    {
        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        DbContext.AddRange(asset, profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var currency = TestData.Currency.Default.Build();

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = new DateTimeOffset(_utcNow),
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id
        };

        var response = await Interactor.Execute(requestModel);

        var notFound = Assert.IsType<NotFound>(response.Value, exactMatch: false);
        Assert.Equal(currency.Id, notFound.ResourceId);
        Assert.Equal(nameof(Currency), notFound.ResourceType);
    }

    [Fact]
    public async Task You_cant_add_a_trade_if_the_profile_does_not_exist()
    {
        var asset = TestData.Asset.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var profile = TestData.Profile.Default.Build();

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = new DateTimeOffset(_utcNow),
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id
        };

        var response = await Interactor.Execute(requestModel);

        var notFound = Assert.IsType<NotFound>(response.Value, exactMatch: false);
        Assert.Equal(profile.Id, notFound.ResourceId);
        Assert.Equal(nameof(Profile), notFound.ResourceType);
    }

    [Fact]
    public async Task You_cant_add_a_trade_if_the_asset_does_not_exist()
    {
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        DbContext.AddRange(currency, profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var asset = TestData.Asset.Default.Build();

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = new DateTimeOffset(_utcNow),
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id
        };

        var response = await Interactor.Execute(requestModel);

        var notFound = Assert.IsType<NotFound>(response.Value, exactMatch: false);
        Assert.Equal(asset.Id, notFound.ResourceId);
        Assert.Equal(nameof(Asset), notFound.ResourceType);
    }

    [Fact]
    public async Task A_trade_can_be_added_successfully()
    {
        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = new DateTimeOffset(_utcNow),
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id
        };

        var response = await Interactor.Execute(requestModel);

        var completed = Assert.IsType<Completed<AddTradeResponseModel>>(response.Value);
        Assert.NotNull(await DbContextSingleOrDefault<Trade>(x => x.Id == completed.Data.TradeId));
    }
    
    [Fact]
    public async Task A_trade_is_not_saved_when_executing_a_dry_run()
    {
        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var requestModel = new AddTradeRequestModel
        {
            DryRun = true,
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = new DateTimeOffset(_utcNow),
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id
        };

        var response = await Interactor.Execute(requestModel);

        var completed = Assert.IsType<Completed<AddTradeResponseModel>>(response.Value);
        Assert.Null(await DbContextSingleOrDefault<Trade>(x => x.Id == completed.Data.TradeId));
    }

    [Fact]
    public async Task Reference_notes_are_successfully_stored()
    {
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = new DateTimeOffset(_utcNow),
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id,
            References =
                [new ReferenceRequestModel(ReferenceType.Other, "https://example.org", "some notes")]
        };

        var response = await Interactor.Execute(requestModel);

        var completed = Assert.IsType<Completed<AddTradeResponseModel>>(response.Value);
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == completed.Data.TradeId);
        Assert.NotNull(newlyAddedTrade?.References);
        Assert.Equal("some notes", newlyAddedTrade.References.Single().Notes);
    }

    [Fact]
    public async Task A_closed_trade_can_be_added_successfully()
    {
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = new DateTimeOffset(_utcNow),
            Closed = new DateTimeOffset(_utcNow),
            ManuallyEnteredResult = ResultModel.Win,
            ProfitLoss = 10,
            EntryPrice = 1.00m,
            ExitPrice = 1.05m,
            Size = 5000,
            CurrencyId = currency.Id
        };

        var response = await Interactor.Execute(requestModel);

        var completed = Assert.IsType<Completed<AddTradeResponseModel>>(response.Value);
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == completed.Data.TradeId);
        Assert.NotNull(newlyAddedTrade);
        Assert.True(newlyAddedTrade.IsClosed);
    }

    [Fact]
    public async Task PositionPrices_must_be_greater_than_zero()
    {
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = new DateTimeOffset(_utcNow),
            Closed = new DateTimeOffset(_utcNow),
            ManuallyEnteredResult = ResultModel.Win,
            ProfitLoss = 10m,
            EntryPrice = 0m,
            StopLoss = 0m,
            TakeProfit = 0m,
            ExitPrice = 0m,
            Size = 5000,
            CurrencyId = currency.Id
        };

        var response = await Interactor.Execute(requestModel);

        var badInput = Assert.IsType<ValidationResult>(response.Value);
        var errors = badInput.Errors;
        Assert.Equal(4, errors.Count);
        Assert.Contains(errors, x => 
            x.PropertyName == "EntryPrice" && x.ErrorMessage == "'Entry Price' must be greater than '0'.");
        Assert.Contains(errors, x =>
            x.PropertyName == "StopLoss" && x.ErrorMessage == "'Stop Loss' must be greater than '0'.");
        Assert.Contains(errors, x =>
            x.PropertyName == "TakeProfit" && x.ErrorMessage == "'Take Profit' must be greater than '0'.");
        Assert.Contains(errors, x =>
            x.PropertyName == "ExitPrice" && x.ErrorMessage == "'Exit Price' must be greater than '0'.");
    }

    [Fact]
    public async Task A_new_closed_trade_without_a_profitLoss_cant_be_closed()
    {
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = new DateTimeOffset(_utcNow),
            Closed = new DateTimeOffset(_utcNow),
            ProfitLoss = null,
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id
        };

        var response = await Interactor.Execute(requestModel);

        var badInput = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(badInput.Errors);
        Assert.Equal("'Profit/Loss' must not be empty, if 'Closed' is specified.", error.ErrorMessage);
        Assert.Equal("ProfitLoss", error.PropertyName);
    }

    [Fact]
    public async Task Opened_passed_as_utc_and_closed_as_local_time_will_both_stored_in_utc()
    {
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var opened = DateTimeOffset.Parse("2024-08-05T14:00:00Z");
        var closed = DateTimeOffset.Parse("2024-08-05T10:00:00-04:00");

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = opened,
            Closed = closed,
            Size = 5000,
            ProfitLoss = 50,
            EntryPrice = 1.05m,
            ExitPrice = 1.15m,
            CurrencyId = currency.Id
        };

        var response = await Interactor.Execute(requestModel);

        var completed = Assert.IsType<Completed<AddTradeResponseModel>>(response.Value);
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == completed.Data.TradeId);
        Assert.NotNull(newlyAddedTrade);
        Assert.Equal(DateTime.Parse("2024-08-05T14:00:00"), newlyAddedTrade.Opened);
        Assert.Equal(DateTime.Parse("2024-08-05T14:00:00"), newlyAddedTrade.Closed);
    }

    [Fact]
    public async Task Specifying_a_manually_entered_result_is_not_possible_if_there_is_no_profitLoss_and_no_closed_date()
    {
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var opened = DateTimeOffset.Parse("2024-08-05T14:00:00Z");

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = opened,
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id,
            ManuallyEnteredResult = ResultModel.Loss
        };

        var response = await Interactor.Execute(requestModel);

        var badInput = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(badInput.Errors);
        Assert.Equal("The result can only be overridden if 'Profit/Loss' and 'Closed' are specified.", 
            error.ErrorMessage);
        Assert.Equal("ManuallyEnteredResult", error.PropertyName);
    }

    [Fact]
    public async Task Specifying_a_manually_entered_result_is_possible_if_profitLoss_and_closed_date_are_present()
    {
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var now = DateTimeOffset.Parse("2024-08-05T14:00:00Z");

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = now,
            Closed = now,
            ProfitLoss = 500m,
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id,
            ManuallyEnteredResult = ResultModel.Loss
        };

        var response = await Interactor.Execute(requestModel);

        var completed = Assert.IsType<Completed<AddTradeResponseModel>>(response.Value);
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == completed.Data.TradeId);
        Assert.NotNull(newlyAddedTrade?.Result);
        Assert.Equal(Result.Loss, newlyAddedTrade.Result.Name);
        Assert.Equal(ResultSource.ManuallyEntered, newlyAddedTrade.Result.Source);
    }
}