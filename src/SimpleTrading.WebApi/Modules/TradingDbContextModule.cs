using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SimpleTrading.DataAccess;

namespace SimpleTrading.WebApi.Modules;

public class TradingDbContextModule(IConfiguration configuration) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var dbProvider = configuration.GetValue<string>("DbProvider")
                         ?? throw new Exception("Missing DbProvider");

        var connectionString = configuration.GetConnectionString(dbProvider)
                               ?? throw new Exception("Missing connection string");

        var dbContextOptions = GetDbContextOptions(dbProvider, connectionString);

        builder.RegisterType<TradingDbContext>()
            .AsSelf()
            .WithParameter(TypedParameter.From(dbContextOptions))
            .InstancePerLifetimeScope();
    }

    private static DbContextOptions<TradingDbContext> GetDbContextOptions(string dbProvider, string connectionString)
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

    private static DbContextOptions<TradingDbContext> GetSqlServerDbContextOptions(string connectionString,
        DbContextOptionsBuilder<TradingDbContext> dbContextOptions)
    {
        const string sqlServerMigrationsAssembly = "SimpleTrading.DataAccess.SqlServer";

        var builder = dbContextOptions.UseSqlServer(connectionString,
            x =>
                x.MigrationsAssembly(sqlServerMigrationsAssembly)
                    .UseAzureSqlDefaults());

        return builder.Options;
    }

    private static DbContextOptions<TradingDbContext> GetPostgresDbContextOptions(string connectionString,
        DbContextOptionsBuilder<TradingDbContext> dbContextOptions)
    {
        const string postgresMigrationsAssembly = "SimpleTrading.DataAccess.Postgres";

        var builder = dbContextOptions.UseNpgsql(connectionString,
            x => x.MigrationsAssembly(postgresMigrationsAssembly));

        return builder.Options;
    }

    private static DbContextOptions<TradingDbContext> GetSqliteDbContextOptions(string connectionString,
        DbContextOptionsBuilder<TradingDbContext> dbContextOptions)
    {
        const string sqliteMigrationsAssembly = "SimpleTrading.DataAccess.Sqlite";

        var builder = dbContextOptions.UseSqlite(connectionString,
            x => x.MigrationsAssembly(sqliteMigrationsAssembly));

        return builder.Options;
    }
}