﻿using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.WebApi;

namespace SimpleTrading.DataAccess.Tests.Repositories;

public class UserSettingsRepositoriesTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-09-17T12:00:00").ToUtcKind();

    protected override void OverrideServices(WebHostBuilderContext ctx, IServiceCollection services)
    {
        services.AddSingleton<UtcNow>(_ => () => _utcNow);
        base.OverrideServices(ctx, services);
    }

    [Fact]
    public async Task LastModified_gets_automatically_updated_if_entity_was_modified()
    {
        // arrange
        var uowCommit = ServiceLocator.GetRequiredService<UowCommit>();
        var userSettingsRepository = ServiceLocator.GetRequiredService<IUserSettingsRepository>();
        var userSettings = await userSettingsRepository.GetUserSettings();
        userSettings.LastModified.Should().NotBe(_utcNow);

        // act
        userSettings.TimeZone = "America/New_York";
        await uowCommit();

        // assert
        var updatedUserSettings = await userSettingsRepository.GetUserSettings();
        updatedUserSettings.LastModified.Should().Be(_utcNow);
    }

    [Fact]
    public async Task LastModified_will_not_be_refreshed_automatically_if_entity_was_just_read()
    {
        // arrange
        var userSettingsRepository = ServiceLocator.GetRequiredService<IUserSettingsRepository>();
        var userSettings = await userSettingsRepository.GetUserSettings();
        var initialUpdatedDate = userSettings.LastModified;

        // act
        var timeZone = userSettings.TimeZone;

        // assert
        var updatedUserSettings = await userSettingsRepository.GetUserSettings();
        updatedUserSettings.LastModified.Should().Be(initialUpdatedDate);
    }
}