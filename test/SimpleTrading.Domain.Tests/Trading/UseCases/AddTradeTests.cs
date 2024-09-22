using System.Globalization;
using Autofac;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.AddTrade;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;
using NotFound = SimpleTrading.Domain.Infrastructure.NotFound;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class AddTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-05T14:00:00").ToUtcKind();

    private IAddTrade Interactor => ServiceLocator.GetRequiredService<IAddTrade>();

    protected override void OverrideServices(HostBuilderContext ctx, ContainerBuilder builder)
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

        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors
            .Should().Contain(x => x.ErrorMessage == "'Asset' must not be empty.")
            .And.Contain(x => x.PropertyName == "AssetId")
            .And.HaveCount(1);
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

        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors
            .Should().Contain(x => x.ErrorMessage == "'Profile' must not be empty.")
            .And.Contain(x => x.PropertyName == "ProfileId")
            .And.HaveCount(1);
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

        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors
            .Should().Contain(x => x.ErrorMessage == "'Currency' must not be empty.")
            .And.Contain(x => x.PropertyName == "CurrencyId")
            .And.HaveCount(1);
    }

    [Theory]
    [InlineData("en-US", "'Trade size' must be greater than '0'.")]
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

        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors
            .Should().Contain(x => x.ErrorMessage == errorMessage)
            .And.Contain(x => x.PropertyName == "Size")
            .And.HaveCount(1);
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
            Balance = 500,
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = TestData.Currency.Default.Build().Id,
            ManuallyEnteredResult = (ResultModel) 50
        };

        var response = await Interactor.Execute(requestModel);

        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors
            .Should().Contain(x => x.ErrorMessage == errorMessage)
            .And.Contain(x => x.PropertyName == "ManuallyEnteredResult")
            .And.HaveCount(1);
    }

    [Theory]
    [InlineData("de-AT", "Der Wert von 'Eröffnet' muss grösser oder gleich '01.01.2000 00:00:00' sein.")]
    [InlineData("en-US", "'Opened' must be greater than or equal to '01.01.2000 00:00:00'.")]
    public async Task Opened_must_not_be_before_min_date(string culture, string errorMessage)
    {
        // arrange
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

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors
            .Should().Contain(x => x.ErrorMessage == errorMessage)
            .And.Contain(x => x.PropertyName == "Opened")
            .And.HaveCount(1);
    }

    [Fact]
    public async Task Opened_must_not_be_greater_than_one_day_in_the_future()
    {
        // arrange
        var opened = new DateTimeOffset(_utcNow.AddDays(1).AddSeconds(1));

        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();

        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = opened,
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id
        };

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors
            .Should().Contain(x => x.ErrorMessage == "'Opened' must be less than or equal to '06.08.2024 16:00'.")
            .And.Contain(x => x.PropertyName == "Opened")
            .And.HaveCount(1);
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

        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors
            .Should().Contain(x => x.ErrorMessage == "Invalid link.")
            .And.Contain(x => x.PropertyName == "References[0].Link")
            .And.HaveCount(1);
    }

    [Theory]
    [InlineData("en-US", "The length of 'Notes' must be 4000 characters or fewer. You entered 40001 characters.")]
    [InlineData("de-AT",
        "Die Länge von 'Anmerkungen' muss kleiner oder gleich 4000 sein. Sie haben 40001 Zeichen eingegeben.")]
    public async Task Reference_notes_with_more_than_4000_chars_are_not_allowed(string culture, string errorMessage)
    {
        // arrange
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

        var reference = new ReferenceRequestModel(ReferenceType.Other,
            "http://example.org",
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

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors
            .Should().Contain(x => x.ErrorMessage == errorMessage)
            .And.Contain(x => x.PropertyName == "References[0].Notes")
            .And.HaveCount(1);
    }

    [Fact]
    public async Task Notes_with_more_than_4000_chars_are_not_allowed()
    {
        // arrange
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

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors
            .Should().Contain(x =>
                x.ErrorMessage ==
                "The length of 'Notes' must be 4000 characters or fewer. You entered 4001 characters.")
            .And.Contain(x => x.PropertyName == "Notes")
            .And.HaveCount(1);
    }

    [Fact]
    public async Task You_cant_add_a_trade_if_the_currency_does_not_exist()
    {
        // arrange
        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        DbContext.AddRange(asset, profile);
        await DbContext.SaveChangesAsync();

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

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        var notFound = response.Value.Should().BeAssignableTo<NotFound>();
        notFound.Which.ResourceId.Should().Be(currency.Id);
        notFound.Which.ResourceType.Should().Be(nameof(Currency));
    }

    [Fact]
    public async Task You_cant_add_a_trade_if_the_profile_does_not_exist()
    {
        // arrange
        var asset = TestData.Asset.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, currency);
        await DbContext.SaveChangesAsync();

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

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        var notFound = response.Value.Should().BeAssignableTo<NotFound>();
        notFound.Which.ResourceId.Should().Be(profile.Id);
        notFound.Which.ResourceType.Should().Be(nameof(Profile));
    }

    [Fact]
    public async Task You_cant_add_a_trade_if_the_asset_does_not_exist()
    {
        // arrange
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        DbContext.AddRange(currency, profile);
        await DbContext.SaveChangesAsync();

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

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        var notFound = response.Value.Should().BeAssignableTo<NotFound>();
        notFound.Which.ResourceId.Should().Be(asset.Id);
        notFound.Which.ResourceType.Should().Be(nameof(Asset));
    }

    [Fact]
    public async Task A_trade_can_be_added_successfully()
    {
        // arrange
        var asset = TestData.Asset.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var currency = TestData.Currency.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = new DateTimeOffset(_utcNow),
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id
        };

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        var newId = response.Value.Should().BeOfType<Completed<Guid>>().Which.Data;
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == newId);

        newlyAddedTrade.Should().NotBeNull();
    }

    [Fact]
    public async Task Reference_notes_are_successfully_stored()
    {
        // arrange
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

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

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        var newId = response.Value.Should().BeOfType<Completed<Guid>>().Which.Data;
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == newId);

        newlyAddedTrade.Should().NotBeNull();
        newlyAddedTrade!.References
            .Should().HaveCount(1)
            .And.Contain(x => x.Notes == "some notes");
    }

    [Fact]
    public async Task A_closed_trade_can_be_added_successfully()
    {
        // arrange
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = new DateTimeOffset(_utcNow),
            Closed = new DateTimeOffset(_utcNow),
            ManuallyEnteredResult = ResultModel.Win,
            Balance = 10,
            EntryPrice = 1.00m,
            ExitPrice = 1.05m,
            Size = 5000,
            CurrencyId = currency.Id
        };

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        var newId = response.Value.Should().BeOfType<Completed<Guid>>().Which.Data;
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == newId);
        newlyAddedTrade.Should().NotBeNull();

        newlyAddedTrade!.IsClosed.Should().BeTrue();
    }

    [Fact]
    public async Task PositionPrices_must_be_greater_than_zero()
    {
        // arrange
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = new DateTimeOffset(_utcNow),
            Closed = new DateTimeOffset(_utcNow),
            ManuallyEnteredResult = ResultModel.Win,
            Balance = 10m,
            EntryPrice = 0m,
            StopLoss = 0m,
            TakeProfit = 0m,
            ExitPrice = 0m,
            Size = 5000,
            CurrencyId = currency.Id
        };

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        var businessError = response.Value.Should().BeOfType<BadInput>();
        businessError.Which.ValidationResult.Errors.Should().HaveCount(4)
            .And.Contain(x =>
                x.PropertyName == "EntryPrice" && x.ErrorMessage == "'Entry price' must be greater than '0'.")
            .And.Contain(x =>
                x.PropertyName == "StopLoss" && x.ErrorMessage == "'Stop loss' must be greater than '0'.")
            .And.Contain(x =>
                x.PropertyName == "TakeProfit" && x.ErrorMessage == "'Take profit' must be greater than '0'.")
            .And.Contain(x =>
                x.PropertyName == "ExitPrice" && x.ErrorMessage == "'Exit price' must be greater than '0'.");
    }

    [Fact]
    public async Task A_new_closed_trade_without_a_balance_cant_be_closed()
    {
        // arrange
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = new DateTimeOffset(_utcNow),
            Closed = new DateTimeOffset(_utcNow),
            Balance = null,
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id
        };

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        var businessError = response.Value.Should().BeOfType<BadInput>();
        businessError.Which.ValidationResult
            .Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Balance" &&
                              x.ErrorMessage == "'Balance' must not be empty, if 'Closed' is specified.");
    }

    [Fact]
    public async Task Opened_passed_as_utc_and_closed_as_local_time_will_both_stored_in_utc()
    {
        // arrange
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        var opened = DateTimeOffset.Parse("2024-08-05T14:00:00Z");
        var closed = DateTimeOffset.Parse("2024-08-05T10:00:00-04:00");

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = opened,
            Closed = closed,
            Size = 5000,
            Balance = 50,
            EntryPrice = 1.05m,
            ExitPrice = 1.15m,
            CurrencyId = currency.Id
        };

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        var newId = response.Value.Should().BeOfType<Completed<Guid>>().Which.Data;
        var newlyAddedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == newId);
        newlyAddedTrade.Should().NotBeNull();

        newlyAddedTrade!.Opened.Should().Be(DateTime.Parse("2024-08-05T14:00:00"));
        newlyAddedTrade.Closed.Should().Be(DateTime.Parse("2024-08-05T14:00:00"));
    }

    [Fact]
    public async Task Specifying_a_manually_entered_result_is_not_possible_if_there_is_no_balance_and_no_closed_date()
    {
        // arrange
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

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

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors
            .Should().HaveCount(1)
            .And.Contain(x =>
                x.ErrorMessage == "The result can only be overridden if 'Balance' and 'Closed' are specified.")
            .And.Contain(x => x.PropertyName == "ManuallyEnteredResult");
    }

    [Fact]
    public async Task Specifying_a_manually_entered_result_is_possible_if_balance_and_closed_date_are_present()
    {
        // arrange
        var currency = TestData.Currency.Default.Build();
        var profile = TestData.Profile.Default.Build();
        var asset = TestData.Asset.Default.Build();
        DbContext.AddRange(asset, profile, currency);
        await DbContext.SaveChangesAsync();

        var now = DateTimeOffset.Parse("2024-08-05T14:00:00Z");

        var requestModel = new AddTradeRequestModel
        {
            AssetId = asset.Id,
            ProfileId = profile.Id,
            Opened = now,
            Closed = now,
            Balance = 500m,
            Size = 5000,
            EntryPrice = 1.05m,
            CurrencyId = currency.Id,
            ManuallyEnteredResult = ResultModel.Loss
        };

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        var completed = response.Value.Should().BeOfType<Completed<Guid>>();
        var addedTradeId = completed.Which.Data;

        var addedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == addedTradeId);
        addedTrade.Should().NotBeNull();
        addedTrade!.Result.Should().NotBeNull();
        addedTrade.Result!.Name.Should().Be(Result.Loss);
        addedTrade.Result.Source.Should().Be(ResultSource.ManuallyEntered);
    }
}