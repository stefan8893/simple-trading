using Microsoft.EntityFrameworkCore;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.AddTrade;

namespace SimpleTrading.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        return services.Scan(scan =>
            scan.FromAssemblyOf<IAddTrade>()
                .AddClasses(f => f.AssignableTo(typeof(IInteractor<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
    }

    public static IServiceCollection AddTradingDbContext(this IServiceCollection services,
        IWebHostEnvironment environment, string connectionString)
    {
        if (environment.IsDevelopment())
            services.AddDbContext<TradingDbContext>(o => o.UseNpgsql(connectionString));
        else
            services.AddDbContext<TradingDbContext>(o => o.UseSqlServer(connectionString));

        services.AddScoped<DbMasterData>();

        return services;
    }

    public static IServiceCollection AddDateTimeProvider(this IServiceCollection services)
    {
        services.AddSingleton<UtcNow>(_ => () => DateTime.UtcNow);
        services.AddSingleton<LocalNow>(sp => async () =>
        {
            var userSettings = await sp.GetRequiredService<TradingDbContext>().GetUserSettings();
            return DateTime.UtcNow.ToLocal(userSettings.TimeZone);
        });

        return services;
    }
}