using Autofac;
using SimpleTrading.WebApi.Clients;

namespace SimpleTrading.WebApi.Modules;

public class WebApiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ClientGenerator>()
            .AsSelf()
            .SingleInstance();
    }
}