using Autofac;
using AwesomeAssertions;
using Microsoft.Extensions.Hosting;
using SimpleTrading.Client;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.User.DataAccess;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.UserSettings.UserSettingsController;

public class UserSettingsControllerTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-04T12:00").ToUtcKind();

    protected override void OverrideServices(HostBuilderContext ctx, ContainerBuilder builder)
    {
        builder.Register<UtcNow>(_ => () => _utcNow);
    }

    [Fact]
    public async Task UserSettings_can_be_retrieved_successfully()
    {
        // arrange
        var profile = (TestData.Profile.Default with {IsSelected = true, Name = "TestProfile"}).Build();
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync();

        var client = await CreateClient();
        var userSettings = await ServiceLocator.Resolve<IUserSettingsRepository>().GetUserSettings();
        userSettings.Culture = "en-US";
        userSettings.TimeZone = "Europe/Vienna";
        userSettings.Language = "de";
        await DbContext.SaveChangesAsync();

        // act
        var userSettingsDto = await client.GetUserSettingsAsync();

        // assert
        userSettingsDto.Culture.Should().Be("en-US");
        userSettingsDto.TimeZone.Should().Be("Europe/Vienna");
        userSettingsDto.Language.Should().Be("de");
        userSettingsDto.SelectedProfileId.Should().Be(profile.Id);
        userSettingsDto.SelectedProfileName.Should().Be(profile.Name);
    }

    [Fact]
    public async Task LastModified_is_correctly_converted()
    {
        // arrange
        var client = await CreateClient();
        var userSettings = await ServiceLocator
            .Resolve<IUserSettingsRepository>()
            .GetUserSettings();

        userSettings.Culture = "en-US";
        userSettings.TimeZone = "Europe/Vienna";
        userSettings.Language = "de";
        await DbContext.SaveChangesAsync();

        // act
        var userSettingsDto = await client.GetUserSettingsAsync();

        // assert
        var nowInLocalTime = _utcNow.ToLocal(userSettings.TimeZone).DateTime;
        userSettingsDto.LastModified.DateTime.Should().Be(nowInLocalTime);
    }

    [Fact]
    public async Task UserSettings_can_be_updated_successfully()
    {
        // arrange
        var client = await CreateClient();
        var userSettings = await ServiceLocator.Resolve<IUserSettingsRepository>().GetUserSettings();
        userSettings.Culture = "en-US";
        userSettings.TimeZone = "Europe/Berlin";
        userSettings.Language = "de";
        await DbContext.SaveChangesAsync();

        // act
        await client.UpdateUserSettingsAsync(new UpdateUserSettingsDto
        {
            Culture = "de-AT",
            IsoLanguageCode = new StringUpdateValue
            {
                Value = "en"
            },
            TimeZone = "Europe/Vienna"
        });

        // assert
        var updatedUserSettings =
            await DbContextSingleOrDefault<Domain.User.UserSettings>(x => x.Id == userSettings.Id);
        updatedUserSettings.Should().NotBeNull();
        updatedUserSettings.Culture.Should().Be("de-AT");
        updatedUserSettings.Language.Should().Be("en");
        updatedUserSettings.TimeZone.Should().Be("Europe/Vienna");
    }

    [Fact]
    public async Task Language_can_be_updated_only_without_changing_other_values()
    {
        // arrange
        var client = await CreateClient();
        var userSettings = await ServiceLocator.Resolve<IUserSettingsRepository>().GetUserSettings();
        userSettings.Culture = "en-US";
        userSettings.TimeZone = "Europe/Berlin";
        userSettings.Language = "de";
        await DbContext.SaveChangesAsync();

        // act
        await client.UpdateUserSettingsAsync(new UpdateUserSettingsDto
        {
            IsoLanguageCode = new StringUpdateValue
            {
                Value = "en"
            }
        });

        // assert
        var updatedUserSettings =
            await DbContextSingleOrDefault<Domain.User.UserSettings>(x => x.Id == userSettings.Id);
        updatedUserSettings.Should().NotBeNull();
        updatedUserSettings.Culture.Should().Be("en-US");
        updatedUserSettings.Language.Should().Be("en");
        updatedUserSettings.TimeZone.Should().Be("Europe/Berlin");
    }
}