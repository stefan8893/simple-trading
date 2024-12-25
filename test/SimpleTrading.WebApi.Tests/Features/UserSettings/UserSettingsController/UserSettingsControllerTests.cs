using Autofac;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
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
        await DbContext.SaveChangesAsync();

        // act
        var userSettingsDto = await client.GetUserSettingsAsync();

        // assert
        userSettingsDto.Culture.Should().Be("en-US");
        userSettingsDto.TimeZone.Should().Be("Europe/Vienna");
        userSettingsDto.Language.Should().Be("de");
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
}