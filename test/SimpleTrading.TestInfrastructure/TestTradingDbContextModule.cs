using Autofac;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SimpleTrading.DataAccess;

namespace SimpleTrading.TestInfrastructure;

public class TestTradingDbContextModule : Module
{
    private const string SqliteMigrationsAssembly = "SimpleTrading.DataAccess.Sqlite";

    protected override void Load(ContainerBuilder builder)
    {
        var connection = new SqliteConnection($"DataSource=file:{Guid.NewGuid()}?mode=memory");
        connection.Open();

        var dbContextOptions = new DbContextOptionsBuilder<TradingDbContext>(
                new DbContextOptions<TradingDbContext>(new Dictionary<Type, IDbContextOptionsExtension>()))
            .UseSqlite(connection,
                x => x.MigrationsAssembly(SqliteMigrationsAssembly))
            .EnableSensitiveDataLogging()
            .Options;

        builder.RegisterType<TradingDbContext>()
            .AsSelf()
            .WithParameter(TypedParameter.From(dbContextOptions))
            .InstancePerLifetimeScope();
    }
}