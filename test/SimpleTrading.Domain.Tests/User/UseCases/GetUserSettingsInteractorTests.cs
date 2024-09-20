using System.Globalization;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.User.DataAccess;
using SimpleTrading.Domain.User.UseCases.GetUserSettings;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.User.UseCases;

public class GetUserSettingsTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private IGetUserSettings Interactor => ServiceLocator.GetRequiredService<IGetUserSettings>();

    [Fact]
    public async Task UserSettings_can_be_retrieved_successfully()
    {
        var userSettingsModel = await Interactor.Execute();

        userSettingsModel.Should().NotBeNull();
    }

    [Fact]
    public async Task Language_is_equal_to_culture_language_if_not_overriden()
    {
        // arrange
        var userSettings = await ServiceLocator.GetRequiredService<IUserSettingsRepository>().GetUserSettings();
        userSettings.Language = null;

        // act
        var userSettingsModel = await Interactor.Execute();

        // assert
        var cultureLanguage = new CultureInfo(userSettingsModel.Culture).TwoLetterISOLanguageName;
        userSettingsModel.Language.Should().Be(cultureLanguage);
    }

    [Fact]
    public async Task Language_is_not_equal_to_culture_language_if_overriden()
    {
        // arrange
        var userSettings = await ServiceLocator.GetRequiredService<IUserSettingsRepository>().GetUserSettings();
        userSettings.Culture = "en-US";
        userSettings.Language = "de";

        // act
        var userSettingsModel = await Interactor.Execute();

        // assert
        userSettingsModel.Language.Should().Be("de");
    }
}