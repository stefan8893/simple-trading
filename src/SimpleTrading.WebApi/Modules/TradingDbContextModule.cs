using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

        var dbContextOptionsBuilder = GetDefaultDbContextOptionsBuilder(dbProvider, connectionString);

        builder.Register<TradingDbContext>(ctx =>
            {
                var utcNow = ctx.Resolve<UtcNow>();
                var loggerFactory = ctx.Resolve<ILoggerFactory>();
                dbContextOptionsBuilder.UseLoggerFactory(loggerFactory);

                return new TradingDbContext(dbContextOptionsBuilder.Options, utcNow);
            })
            .InstancePerLifetimeScope();
    }

    private static DbContextOptionsBuilder<TradingDbContext> GetDefaultDbContextOptionsBuilder(string dbProvider,
        string connectionString)
    {
        var dbContextOptions = new DbContextOptionsBuilder<TradingDbContext>(
            new DbContextOptions<TradingDbContext>(new Dictionary<Type, IDbContextOptionsExtension>()));

        if (dbProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            return UseSqlServerDbContextOptions(connectionString, dbContextOptions);

        if (dbProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
            return UsePostgresDbContextOptions(connectionString, dbContextOptions);

        if (dbProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
            return UseSqliteDbContextOptions(connectionString, dbContextOptions);

        throw new Exception("Unknown db provider");
    }

    private static DbContextOptionsBuilder<TradingDbContext> UseSqlServerDbContextOptions(string connectionString,
        DbContextOptionsBuilder<TradingDbContext> dbContextOptions)
    {
        const string sqlServerMigrationsAssembly = "SimpleTrading.DataAccess.SqlServer";

        return dbContextOptions.UseAzureSql(connectionString,
            x => x.MigrationsAssembly(sqlServerMigrationsAssembly));
    }

    private static DbContextOptionsBuilder<TradingDbContext> UsePostgresDbContextOptions(string connectionString,
        DbContextOptionsBuilder<TradingDbContext> dbContextOptions)
    {
        const string postgresMigrationsAssembly = "SimpleTrading.DataAccess.Postgres";

        return dbContextOptions.UseNpgsql(connectionString,
            x => x.MigrationsAssembly(postgresMigrationsAssembly));
    }

    private static DbContextOptionsBuilder<TradingDbContext> UseSqliteDbContextOptions(string connectionString,
        DbContextOptionsBuilder<TradingDbContext> dbContextOptions)
    {
        const string sqliteMigrationsAssembly = "SimpleTrading.DataAccess.Sqlite";

        return dbContextOptions.UseSqlite(connectionString,
            x => x.MigrationsAssembly(sqliteMigrationsAssembly));
    }
}