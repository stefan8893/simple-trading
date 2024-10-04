using Autofac;
using FluentValidation;
using SimpleTrading.WebApi.Clients;

namespace SimpleTrading.WebApi.Modules;

public class WebApiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var webApiAssembly = typeof(Program).Assembly;
        
        builder.RegisterType<ClientGenerator>()
            .AsSelf()
            .SingleInstance();
        
        builder.RegisterAssemblyTypes(webApiAssembly)
            .AsClosedTypesOf(typeof(IValidator<>))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}