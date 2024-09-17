using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.TestInfrastructure;

namespace SimpleTrading.WebApi.Tests.Features.UserSettings.UserSettingsController;

public class UserSettingsControllerTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-04T12:00").ToUtcKind();

    protected override void OverrideServices(WebHostBuilderContext ctx, IServiceCollection services)
    {
        services.AddSingleton<UtcNow>(sp => () => _utcNow);
        base.OverrideServices(ctx, services);
    }

    [Fact]
    public async Task UserSettings_can_be_retrieved_successfully()
    {
        // arrange
        var client = await CreateClient();
        var userSettings = await ServiceLocator.GetRequiredService<IUserSettingsRepository>().GetUserSettings();
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
        var userSettings = await ServiceLocator.GetRequiredService<IUserSettingsRepository>().GetUserSettings();
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