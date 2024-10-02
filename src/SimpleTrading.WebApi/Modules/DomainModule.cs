﻿using Autofac;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.User.DataAccess;
using SimpleTrading.Domain.User.Factories;

namespace SimpleTrading.WebApi.Modules;

public class DomainModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        AddDateTimeProviders(builder);
        
        var assemblyThatContainsUseCases = typeof(IInteractor<>).Assembly;

        builder.RegisterAssemblyTypes(assemblyThatContainsUseCases)
            .AsClosedTypesOf(typeof(IInteractor<,>))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(assemblyThatContainsUseCases)
            .AsClosedTypesOf(typeof(IInteractor<>))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
        
        builder.RegisterGenericDecorator(typeof(InteractorLoggingDecorator<,>), typeof(IInteractor<,>));
        builder.RegisterGenericDecorator(typeof(InteractorLoggingDecorator<>), typeof(IInteractor<>));

        builder.RegisterAssemblyTypes(assemblyThatContainsUseCases)
            .Where(x => x.Name.EndsWith("InteractorProxy"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }

    private static void AddDateTimeProviders(ContainerBuilder builder)
    {
        builder.Register<UtcNow>(_ => () => DateTime.UtcNow)
            .SingleInstance();

        builder.Register<LocalNow>(ctx =>
                DateTimeProviderFactory.CreateLocalNowFunc(ctx.Resolve<IUserSettingsRepository>()))
            .InstancePerLifetimeScope();
    }
}