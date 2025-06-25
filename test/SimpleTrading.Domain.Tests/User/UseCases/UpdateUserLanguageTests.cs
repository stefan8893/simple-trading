using Autofac;
using AwesomeAssertions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.User;
using SimpleTrading.Domain.User.DataAccess;
using SimpleTrading.Domain.User.UseCases.UpdateUserLanguage;
using SimpleTrading.TestInfrastructure;

namespace SimpleTrading.Domain.Tests.User.UseCases;

public class UpdateUserLanguageTests : DomainTests
{
    private IUpdateUserLanguage Interactor => ServiceLocator.Resolve<IUpdateUserLanguage>();

    [Fact]
    public async Task Initial_user_lang_is_en_and_gets_set_to_null()
    {
        // arrange
        var userSettings = await ServiceLocator
            .Resolve<IUserSettingsRepository>()
            .GetUserSettings();

        userSettings.Language = "en";
        await DbContext.SaveChangesAsync();

        var requestModel = new UpdateUserLanguageRequestModel(null);

        // act
        var userSettingsModel = await Interactor.Execute(requestModel);

        // assert
        userSettingsModel.Value.Should().BeOfType<Completed>();
        var userSettingsUpdated = await DbContextSingleOrDefault<UserSettings>(x => x.Id == userSettings.Id);
        userSettingsUpdated.Should().NotBeNull();
        userSettingsUpdated.Language.Should().BeNull();
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

        var requestModel = new UpdateUserLanguageRequestModel("de");

        // act
        var userSettingsModel = await Interactor.Execute(requestModel);

        // assert
        userSettingsModel.Value.Should().BeOfType<Completed>();
        var userSettingsUpdated = await DbContextSingleOrDefault<UserSettings>(x => x.Id == userSettings.Id);
        userSettingsUpdated.Should().NotBeNull();
        userSettingsUpdated.Language.Should().Be("de");
    }

    [Fact]
    public async Task A_three_letter_lang_code_is_not_accepted()
    {
        // arrange
        var requestModel = new UpdateUserLanguageRequestModel("deu");

        // act
        var userSettingsModel = await Interactor.Execute(requestModel);

        // assert
        userSettingsModel.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors.Should()
            .Contain(x =>
                x.PropertyName == "IsoLanguageCode" && 
                x.ErrorMessage == "'deu' is not supported. Available languages are 'de, en'.").And
            .HaveCount(1);
    }
}