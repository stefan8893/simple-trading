using Autofac;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.User;
using SimpleTrading.Domain.User.DataAccess;
using SimpleTrading.Domain.User.UseCases.UpdateUserSettings;
using SimpleTrading.TestInfrastructure;

namespace SimpleTrading.Domain.Tests.User.UseCases;

public class UpdateUserSettingsTests : DomainTests
{
    private IUpdateUserSettings Interactor => ServiceLocator.Resolve<IUpdateUserSettings>();

    [Fact]
    public async Task Initial_user_lang_is_en_and_gets_set_to_null()
    {
        // arrange
        var userSettings = await ServiceLocator
            .Resolve<IUserSettingsRepository>()
            .GetUserSettings();

        userSettings.Language = "en";
        await DbContext.SaveChangesAsync();

        var requestModel = new UpdateUserSettingsRequestModel(null, null, null);

        // act
        var userSettingsModel = await Interactor.Execute(requestModel);

        // assert
        Assert.IsType<Completed>(userSettingsModel.Value);
        var userSettingsUpdated = await DbContextSingleOrDefault<UserSettings>(x => x.Id == userSettings.Id);
        Assert.NotNull(userSettingsUpdated);
        Assert.Null(userSettingsUpdated.Language);
    }

    [Fact]
    public async Task You_can_update_all_values_at_once()
    {
        // arrange
        var userSettings = await ServiceLocator
            .Resolve<IUserSettingsRepository>()
            .GetUserSettings();

        userSettings.Culture = "de-AT";
        userSettings.Language = "en";
        userSettings.TimeZone = "America/Los_Angeles";
        await DbContext.SaveChangesAsync();

        var requestModel = new UpdateUserSettingsRequestModel("de-AT", "de", "Europe/Vienna");

        // act
        var userSettingsModel = await Interactor.Execute(requestModel);

        // assert
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
        // arrange
        var userSettings = await ServiceLocator
            .Resolve<IUserSettingsRepository>()
            .GetUserSettings();

        userSettings.Language = "en";
        await DbContext.SaveChangesAsync();

        var requestModel = new UpdateUserSettingsRequestModel(null, "de", null);

        // act
        var userSettingsModel = await Interactor.Execute(requestModel);

        // assert
        Assert.IsType<Completed>(userSettingsModel.Value);
        var userSettingsUpdated = await DbContextSingleOrDefault<UserSettings>(x => x.Id == userSettings.Id);
        Assert.NotNull(userSettingsUpdated);
        Assert.Equal("de", userSettingsUpdated.Language);
    }

    [Fact]
    public async Task A_three_letter_lang_code_is_not_accepted()
    {
        // arrange
        var requestModel = new UpdateUserSettingsRequestModel(null, "deu", null);

        // act
        var userSettingsModel = await Interactor.Execute(requestModel);

        // assert
        var badInput = Assert.IsType<BadInput>(userSettingsModel.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("IsoLanguageCode", error.PropertyName);
        Assert.Equal("'deu' is not supported. Only 'de, en'.", error.ErrorMessage);
    }

    [Fact]
    public async Task Only_supported_cultures_are_allowed()
    {
        // arrange
        const string notSupportedCulture = "de-CH";
        var requestModel = new UpdateUserSettingsRequestModel(notSupportedCulture, null, null);

        // act
        var userSettingsModel = await Interactor.Execute(requestModel);

        // assert
        var badInput = Assert.IsType<BadInput>(userSettingsModel.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("Culture", error.PropertyName);
        Assert.Equal("'de-CH' is not supported. Only 'de-AT, en-US'.", error.ErrorMessage);
    }

    [Fact]
    public async Task Timezone_must_be_a_known_iana_zone()
    {
        // arrange
        const string notSupportedTimezone = "Europe/Bregenz";
        var requestModel = new UpdateUserSettingsRequestModel(null, null, notSupportedTimezone);

        // act
        var userSettingsModel = await Interactor.Execute(requestModel);

        // assert
        var badInput = Assert.IsType<BadInput>(userSettingsModel.Value);
        var error = Assert.Single(badInput.ValidationResult.Errors);
        Assert.Equal("Timezone", error.PropertyName);
        Assert.Equal("'Europe/Bregenz' is invalid.", error.ErrorMessage);
    }
}