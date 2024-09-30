using Autofac;
using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.WebApi.Modules;

public class DomainModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var assemblyThatContainsUseCases = typeof(IInteractor<>).Assembly;

        builder.RegisterAssemblyTypes(assemblyThatContainsUseCases)
            .AsClosedTypesOf(typeof(IInteractor<,>))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(assemblyThatContainsUseCases)
            .AsClosedTypesOf(typeof(IInteractor<>))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(assemblyThatContainsUseCases)
            .Where(x => x.Name.EndsWith("InteractorProxy"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}