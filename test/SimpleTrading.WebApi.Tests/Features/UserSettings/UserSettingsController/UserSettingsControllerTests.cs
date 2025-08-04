using Autofac;
using Microsoft.Extensions.Hosting;
using SimpleTrading.Client;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.User.DataAccess;
using SimpleTrading.TestInfrastructure;

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
        var client = await CreateClient();

        var userSettings = await ServiceLocator.Resolve<IUserSettingsRepository>().GetUserSettings();
        userSettings.Culture = "en-US";
        userSettings.TimeZone = "Europe/Vienna";
        userSettings.Language = "de";
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var userSettingsDto = await client.GetUserSettingsAsync(TestContext.Current.CancellationToken);

        // assert
        Assert.Equal("en-US", userSettingsDto.Culture);
        Assert.Equal("Europe/Vienna", userSettingsDto.TimeZone);
        Assert.Equal("de", userSettingsDto.Language);
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
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var userSettingsDto = await client.GetUserSettingsAsync(TestContext.Current.CancellationToken);

        // assert
        var nowInLocalTime = _utcNow.ToLocal(userSettings.TimeZone).DateTime;
        Assert.Equal(nowInLocalTime, userSettingsDto.LastModified.DateTime);
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
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        await client.UpdateUserSettingsAsync(new UpdateUserSettingsDto
        {
            Culture = "de-AT",
            IsoLanguageCode = new UpdateStringValue
            {
                Value = "en"
            },
            TimeZone = "Europe/Vienna"
        }, TestContext.Current.CancellationToken);

        // assert
        var updatedUserSettings =
            await DbContextSingleOrDefault<Domain.User.UserSettings>(x => x.Id == userSettings.Id);
        Assert.NotNull(updatedUserSettings);
        Assert.Equal("de-AT", updatedUserSettings.Culture);
        Assert.Equal("en", updatedUserSettings.Language);
        Assert.Equal("Europe/Vienna", updatedUserSettings.TimeZone);
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
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        await client.UpdateUserSettingsAsync(new UpdateUserSettingsDto
        {
            IsoLanguageCode = new UpdateStringValue
            {
                Value = "en"
            }
        }, TestContext.Current.CancellationToken);

        // assert
        var updatedUserSettings =
            await DbContextSingleOrDefault<Domain.User.UserSettings>(x => x.Id == userSettings.Id);
        Assert.NotNull(updatedUserSettings);
        Assert.Equal("en-US", updatedUserSettings.Culture);
        Assert.Equal("en", updatedUserSettings.Language);
        Assert.Equal("Europe/Berlin", updatedUserSettings.TimeZone);
    }
}