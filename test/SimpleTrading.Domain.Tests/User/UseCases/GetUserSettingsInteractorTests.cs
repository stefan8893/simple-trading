using Autofac;
using SimpleTrading.Domain.User.DataAccess;
using SimpleTrading.Domain.User.UseCases.GetUserSettings;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.User.UseCases;

public class GetUserSettingsTests : DomainTests
{
    private IGetUserSettings Interactor => ServiceLocator.Resolve<IGetUserSettings>();

    [Fact]
    public async Task UserSettings_can_be_retrieved_successfully()
    {
        var profile = (TestData.Profile.Default with {IsActive = true, Name = "TestProfile"}).Build();
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var userSettingsModel = await Interactor.Execute(TestContext.Current.CancellationToken);

        Assert.NotNull(userSettingsModel);
    }

    [Fact]
    public async Task Language_can_be_null()
    {
        var profile = (TestData.Profile.Default with {IsActive = true, Name = "TestProfile"}).Build();
        DbContext.Profiles.Add(profile);

        var userSettings = await ServiceLocator
            .Resolve<IUserSettingsRepository>()
            .GetUserSettings();

        userSettings.Language = null;
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var userSettingsModel = await Interactor.Execute(TestContext.Current.CancellationToken);

        Assert.Null(userSettingsModel.Language);
    }

    [Fact]
    public async Task Language_is_not_equal_to_culture_language_if_overriden()
    {
        var profile = (TestData.Profile.Default with {IsActive = true, Name = "TestProfile"}).Build();
        DbContext.Profiles.Add(profile);

        var userSettings = await ServiceLocator
            .Resolve<IUserSettingsRepository>()
            .GetUserSettings();

        userSettings.Culture = "en-US";
        userSettings.Language = "de";
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var userSettingsModel = await Interactor.Execute(TestContext.Current.CancellationToken);

        Assert.Equal("de", userSettingsModel.Language);
    }
}