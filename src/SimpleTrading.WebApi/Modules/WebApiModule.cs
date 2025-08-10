using System.Reflection;
using Autofac;
using FluentValidation;
using SimpleTrading.WebApi.Clients;
using SimpleTrading.WebApi.Infrastructure;
using Module = Autofac.Module;

namespace SimpleTrading.WebApi.Modules;

public class WebApiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        AddClientGenerator(builder);
        AddSimpleProblemDetails(builder);

        var webApiAssembly = typeof(Program).Assembly;
        AddValidators(builder, webApiAssembly);
    }

    private static void AddClientGenerator(ContainerBuilder builder)
    {
        builder.RegisterType<ClientGenerator>()
            .AsSelf()
            .SingleInstance();
    }

    private static void AddSimpleProblemDetails(ContainerBuilder builder)
    {
        builder.RegisterType<SimpleProblemDetails>()
            .AsSelf()
            .InstancePerLifetimeScope();
    }

    private static void AddValidators(ContainerBuilder builder, Assembly webApiAssembly)
    {
        builder.RegisterAssemblyTypes(webApiAssembly)
            .AsClosedTypesOf(typeof(IValidator<>))
            .InstancePerLifetimeScope();
    }
}