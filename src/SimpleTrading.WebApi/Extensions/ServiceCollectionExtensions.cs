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
        IConfiguration configuration)
    {
        var dbProvider = configuration.GetValue<string>("DbProvider")
                         ?? throw new Exception("Missing DbProvider");

        var connectionString = configuration.GetConnectionString(dbProvider);

        const string sqlServerMigrationsAssembly = "SimpleTrading.DataAccess.SqlServer";
        const string postgresMigrationsAssembly = "SimpleTrading.DataAccess.Postgres";
        const string sqliteMigrationsAssembly = "SimpleTrading.DataAccess.Sqlite";

        var dbContextOptionsBuilderByProvider =
            new Dictionary<string, Action<DbContextOptionsBuilder>>(StringComparer.OrdinalIgnoreCase)
            {
                ["SqlServer"] = o =>
                {
                    o.UseSqlServer(connectionString,
                        x => x.MigrationsAssembly(sqlServerMigrationsAssembly));
                },
                ["Postgres"] = o =>
                {
                    o.UseNpgsql(connectionString,
                        x => x.MigrationsAssembly(postgresMigrationsAssembly));
                },
                ["Sqlite"] = o =>
                {
                    o.UseSqlite(connectionString,
                        x => x.MigrationsAssembly(sqliteMigrationsAssembly));
                }
            };

        services.AddDbContext<TradingDbContext>(dbContextOptionsBuilderByProvider[dbProvider]);
        services.AddScoped<DbMasterData>();

        return services;
    }

    public static IServiceCollection AddDateTimeProvider(this IServiceCollection services)
    {
        services.AddSingleton<UtcNow>(_ => () => DateTime.UtcNow);
        services.AddSingleton<LocalNow>(sp => async () =>
        {
            using var scope = sp.CreateScope();
            var userSettings = await scope.ServiceProvider
                .GetRequiredService<TradingDbContext>()
                .GetUserSettings();
            
            return DateTime.UtcNow.ToLocal(userSettings.TimeZone).DateTime.ToLocalKind();
        });

        return services;
    }
}