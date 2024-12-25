using System.Reflection;
using Autofac;
using FluentValidation;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.User.DataAccess;
using SimpleTrading.Domain.User.Factories;
using Module = Autofac.Module;

namespace SimpleTrading.WebApi.Modules;

public class DomainModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        AddDateTimeProviders(builder);

        var domainAssembly = typeof(IInteractor<>).Assembly;
        AddInteractors(builder, domainAssembly);
        AddInteractorProxies(builder, domainAssembly);
        AddValidators(builder, domainAssembly);
    }

    private static void AddDateTimeProviders(ContainerBuilder builder)
    {
        builder.Register<UtcNow>(_ => () => DateTime.UtcNow)
            .SingleInstance();

        builder.Register<LocalNow>(ctx =>
                DateTimeProviderFactory.CreateLocalNowFunc(ctx.Resolve<IUserSettingsRepository>()))
            .InstancePerLifetimeScope();
    }

    private static void AddInteractors(ContainerBuilder builder, Assembly domainAssembly)
    {
        builder.RegisterAssemblyTypes(domainAssembly)
            .AsClosedTypesOf(typeof(IInteractor<,>))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(domainAssembly)
            .AsClosedTypesOf(typeof(IInteractor<>))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterGenericDecorator(typeof(InteractorLoggingDecorator<,>), typeof(IInteractor<,>));
        builder.RegisterGenericDecorator(typeof(InteractorLoggingDecorator<>), typeof(IInteractor<>));
    }

    private static void AddInteractorProxies(ContainerBuilder builder, Assembly domainAssembly)
    {
        builder.RegisterAssemblyTypes(domainAssembly)
            .Where(x => x.Name.EndsWith("InteractorProxy"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }

    private static void AddValidators(ContainerBuilder builder, Assembly domainAssembly)
    {
        builder.RegisterAssemblyTypes(domainAssembly)
            .AsClosedTypesOf(typeof(IValidator<>))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}