using System.Linq.Expressions;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.WebApi.Modules;

namespace SimpleTrading.DataAccess.Tests;

public abstract class RepositoryTests
    : TestBase, IAsyncLifetime
{
    private IContainer? _container;
    private TradingDbContext? _dbContext;
    private ILifetimeScope? _lifetimeScope;

    protected TradingDbContext DbContext => _dbContext!;

    protected ILifetimeScope ServiceLocator => _lifetimeScope!;

    public async ValueTask DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        _lifetimeScope?.Dispose();
        _container?.Dispose();
    }

    public async ValueTask InitializeAsync()
    {
        var containerBuilder = new ContainerBuilder();
        PrepareContainer(containerBuilder);
        OverrideServices(containerBuilder);
        _container = containerBuilder.Build();

        _lifetimeScope = _container.BeginLifetimeScope();
        _dbContext = _lifetimeScope.Resolve<TradingDbContext>();

        await DbContext.Database.MigrateAsync();

        var dbMasterData = _lifetimeScope.Resolve<DbMasterData>();
        await dbMasterData.Seed();
    }

    private static void PrepareContainer(ContainerBuilder builder)
    {
        builder.RegisterModule<TestTradingDbContextModule>();
        builder.RegisterModule<DomainModule>();
        builder.RegisterModule<DataAccessModule>();

        builder.RegisterType<NullLoggerFactory>()
            .As<ILoggerFactory>()
            .SingleInstance();
    }

    /// <summary>
    ///     Will be invoked before each test run
    /// </summary>
    /// <param name="builder"></param>
    protected virtual void OverrideServices(ContainerBuilder builder)
    {
    }

    protected Task<T?> DbContextSingleOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
    {
        return DbContext.Set<T>()
            .AsNoTracking()
            .SingleOrDefaultAsync(predicate);
    }
}