using Autofac;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.User.DataAccess;

namespace SimpleTrading.DataAccess.Tests.Repositories;

public class UserSettingsRepositoriesTests : RepositoryTests
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-09-17T12:00:00").ToUtcKind();

    protected override void OverrideServices(ContainerBuilder builder)
    {
        builder.Register<UtcNow>(_ => () => _utcNow);
    }

    [Fact]
    public async Task LastModified_gets_automatically_updated_if_entity_was_modified()
    {
        // arrange
        var uowCommit = ServiceLocator.Resolve<UowCommit>();
        var userSettingsRepository = ServiceLocator.Resolve<IUserSettingsRepository>();
        var userSettings = await userSettingsRepository.GetUserSettings();
        Assert.NotEqual(_utcNow, userSettings.LastModified);

        // act
        userSettings.TimeZone = "America/New_York";
        await uowCommit();

        // assert
        var updatedUserSettings = await userSettingsRepository.GetUserSettings();
        Assert.Equal(_utcNow, updatedUserSettings.LastModified);
    }

    [Fact]
    public async Task LastModified_will_not_be_refreshed_automatically_if_entity_was_just_read()
    {
        // arrange
        var userSettingsRepository = ServiceLocator.Resolve<IUserSettingsRepository>();
        var userSettings = await userSettingsRepository.GetUserSettings();
        var initialUpdatedDate = userSettings.LastModified;

        // act
        _ = userSettings.TimeZone;

        // assert
        var updatedUserSettings = await userSettingsRepository.GetUserSettings();
        Assert.Equal(initialUpdatedDate, updatedUserSettings.LastModified);
    }
}