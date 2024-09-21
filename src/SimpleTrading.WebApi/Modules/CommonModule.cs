using Autofac;
using SimpleTrading.WebApi.Clients;

namespace SimpleTrading.WebApi.Modules;

public class CommonModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ClientGenerator>()
            .AsSelf()
            .SingleInstance();
    }
}