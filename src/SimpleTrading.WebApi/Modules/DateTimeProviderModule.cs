using Autofac;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.User.DataAccess;
using SimpleTrading.Domain.User.Factories;

namespace SimpleTrading.WebApi.Modules;

public class DateTimeProviderModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register<UtcNow>(_ => () => DateTime.UtcNow)
            .SingleInstance();

        builder.Register<LocalNow>(ctx =>
                DateTimeProviderFactory.CreateLocalNowFunc(ctx.Resolve<IUserSettingsRepository>()))
            .InstancePerLifetimeScope();
    }
}