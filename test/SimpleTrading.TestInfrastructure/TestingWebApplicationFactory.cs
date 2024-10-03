using Autofac;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SimpleTrading.TestInfrastructure;

[UsedImplicitly]
public class TestingWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public Action<HostBuilderContext, ContainerBuilder>? OverrideServices { get; set; }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureContainer<ContainerBuilder>((ctx, b) =>
        {
            b.RegisterModule(new TestTradingDbContextModule());
            OverrideServices?.Invoke(ctx, b);
        });

        return base.CreateHost(builder);
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureLogging(lb => { lb.SetMinimumLevel(LogLevel.Error); });
    }
}