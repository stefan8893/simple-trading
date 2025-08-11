using Autofac;
using FluentValidation.Results;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.User;
using SimpleTrading.Domain.User.DataAccess;
using SimpleTrading.Domain.User.UseCases.UpdateUserSettings;

namespace SimpleTrading.Domain.Tests.User.UseCases;

public class UpdateUserSettingsTests : DomainTests
{
    private IUpdateUserSettings Interactor => ServiceLocator.Resolve<IUpdateUserSettings>();

    [Fact]
    public async Task Initial_user_lang_is_en_and_gets_set_to_null()
    {
        var userSettings = await ServiceLocator
            .Resolve<IUserSettingsRepository>()
            .GetUserSettings();

        userSettings.Language = "en";
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var requestModel = new UpdateUserSettingsRequestModel(null, null, null);

        var userSettingsModel = await Interactor.Execute(requestModel);

        Assert.IsType<Completed>(userSettingsModel.Value);
        var userSettingsUpdated = await DbContextSingleOrDefault<UserSettings>(x => x.Id == userSettings.Id);
        Assert.NotNull(userSettingsUpdated);
        Assert.Null(userSettingsUpdated.Language);
    }

    [Fact]
    public async Task You_can_update_all_values_at_once()
    {
        var userSettings = await ServiceLocator
            .Resolve<IUserSettingsRepository>()
            .GetUserSettings();

        userSettings.Culture = "de-AT";
        userSettings.Language = "en";
        userSettings.TimeZone = "America/Los_Angeles";
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var requestModel = new UpdateUserSettingsRequestModel("de-AT", "de", "Europe/Vienna");

        var userSettingsModel = await Interactor.Execute(requestModel);

        Assert.IsType<Completed>(userSettingsModel.Value);
        var userSettingsUpdated = await DbContextSingleOrDefault<UserSettings>(x => x.Id == userSettings.Id);
        Assert.NotNull(userSettingsUpdated);
        Assert.Equal("de-AT", userSettingsUpdated.Culture);
        Assert.Equal("de", userSettingsUpdated.Language);
        Assert.Equal("Europe/Vienna", userSettingsUpdated.TimeZone);
    }

    [Fact]
    public async Task Initial_user_lang_is_en_and_gets_set_to_de()
    {
        var userSettings = await ServiceLocator
            .Resolve<IUserSettingsRepository>()
            .GetUserSettings();

        userSettings.Language = "en";
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var requestModel = new UpdateUserSettingsRequestModel(null, "de", null);

        var userSettingsModel = await Interactor.Execute(requestModel);

        Assert.IsType<Completed>(userSettingsModel.Value);
        var userSettingsUpdated = await DbContextSingleOrDefault<UserSettings>(x => x.Id == userSettings.Id);
        Assert.NotNull(userSettingsUpdated);
        Assert.Equal("de", userSettingsUpdated.Language);
    }

    [Fact]
    public async Task A_three_letter_lang_code_is_not_accepted()
    {
        var requestModel = new UpdateUserSettingsRequestModel(null, "deu", null);

        var userSettingsModel = await Interactor.Execute(requestModel);

        var validationResult = Assert.IsType<ValidationResult>(userSettingsModel.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("IsoLanguageCode", error.PropertyName);
        Assert.Equal("'DEU' is not supported. Only 'DE, EN'.", error.ErrorMessage);
    }

    [Fact]
    public async Task Only_supported_cultures_are_allowed()
    {
        const string notSupportedCulture = "de-CH";
        var requestModel = new UpdateUserSettingsRequestModel(notSupportedCulture, null, null);

        var userSettingsModel = await Interactor.Execute(requestModel);

        var validationResult = Assert.IsType<ValidationResult>(userSettingsModel.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("Culture", error.PropertyName);
        Assert.Equal("'de-CH' is not supported. Only 'de-AT, en-US'.", error.ErrorMessage);
    }

    [Fact]
    public async Task Timezone_must_be_a_known_iana_zone()
    {
        const string notSupportedTimezone = "Europe/Bregenz";
        var requestModel = new UpdateUserSettingsRequestModel(null, null, notSupportedTimezone);

        var userSettingsModel = await Interactor.Execute(requestModel);

        var validationResult = Assert.IsType<ValidationResult>(userSettingsModel.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("Timezone", error.PropertyName);
        Assert.Equal("'Europe/Bregenz' is invalid.", error.ErrorMessage);
    }
}