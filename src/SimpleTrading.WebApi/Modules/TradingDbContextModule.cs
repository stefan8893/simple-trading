using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Serilog.Extensions.Logging;
using SimpleTrading.DataAccess;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.WebApi.Modules;

public class TradingDbContextModule(IConfiguration configuration) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var dbProvider = configuration.GetValue<string>("DbProvider")
                         ?? throw new Exception("Missing DbProvider");

        var connectionString = configuration.GetConnectionString(dbProvider)
                               ?? throw new Exception("Missing connection string");

        var dbContextOptionsBuilder = GetDbContextOptionsBuilder(dbProvider, connectionString)
                .UseLoggerFactory(new SerilogLoggerFactory());

        builder.Register<TradingDbContext>(ctx =>
        {
            var utcNow = ctx.Resolve<UtcNow>();
            var loggerFactory  = ctx.Resolve<ILoggerFactory>();
            dbContextOptionsBuilder.UseLoggerFactory(loggerFactory);

            return new TradingDbContext(dbContextOptionsBuilder.Options, utcNow);
        })
        .InstancePerLifetimeScope();
    }

    private static DbContextOptionsBuilder<TradingDbContext> GetDbContextOptionsBuilder(string dbProvider, string connectionString)
    {
        var dbContextOptions = new DbContextOptionsBuilder<TradingDbContext>(
            new DbContextOptions<TradingDbContext>(new Dictionary<Type, IDbContextOptionsExtension>()));

        if (dbProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            return GetSqlServerDbContextOptions(connectionString, dbContextOptions);

        if (dbProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
            return GetPostgresDbContextOptions(connectionString, dbContextOptions);

        if (dbProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
            return GetSqliteDbContextOptions(connectionString, dbContextOptions);

        throw new Exception("Unknown db provider");
    }

    private static DbContextOptionsBuilder<TradingDbContext> GetSqlServerDbContextOptions(string connectionString,
        DbContextOptionsBuilder<TradingDbContext> dbContextOptions)
    {
        const string sqlServerMigrationsAssembly = "SimpleTrading.DataAccess.SqlServer";

        var builder = dbContextOptions.UseSqlServer(connectionString,
            x =>
                x.MigrationsAssembly(sqlServerMigrationsAssembly)
                    .UseAzureSqlDefaults());

        return builder;
    }

    private static DbContextOptionsBuilder<TradingDbContext> GetPostgresDbContextOptions(string connectionString,
        DbContextOptionsBuilder<TradingDbContext> dbContextOptions)
    {
        const string postgresMigrationsAssembly = "SimpleTrading.DataAccess.Postgres";

        var builder = dbContextOptions.UseNpgsql(connectionString,
            x => x.MigrationsAssembly(postgresMigrationsAssembly));

        return builder;
    }

    private static DbContextOptionsBuilder<TradingDbContext> GetSqliteDbContextOptions(string connectionString,
        DbContextOptionsBuilder<TradingDbContext> dbContextOptions)
    {
        const string sqliteMigrationsAssembly = "SimpleTrading.DataAccess.Sqlite";

        var builder = dbContextOptions.UseSqlite(connectionString,
            x => x.MigrationsAssembly(sqliteMigrationsAssembly));

        return builder;
    }
}