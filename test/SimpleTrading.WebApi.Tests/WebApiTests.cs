using System.Linq.Expressions;
using System.Net.Http.Headers;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleTrading.Client;
using SimpleTrading.DataAccess;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.Authentication;

namespace SimpleTrading.WebApi.Tests;

public abstract class WebApiTests(TestingWebApplicationFactory<Program> factory)
    : TestBase, IClassFixture<TestingWebApplicationFactory<Program>>, IAsyncLifetime
{
    private TradingDbContext? _dbContext;
    private ILifetimeScope? _lifetimeScope;
    private IServiceScope? _serviceScope;

    protected TradingDbContext DbContext => _dbContext!;

    protected ILifetimeScope ServiceLocator => _lifetimeScope!;

    public async ValueTask DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        _serviceScope?.Dispose();
    }

    public async ValueTask InitializeAsync()
    {
        factory.OverrideServices = OverrideServices;
        _serviceScope = factory.Services.CreateScope();
        _lifetimeScope = _serviceScope.ServiceProvider.GetRequiredService<ILifetimeScope>();
        _dbContext = _serviceScope.ServiceProvider.GetRequiredService<TradingDbContext>();

        await DbContext.Database.MigrateAsync();

        var dbMasterData = _serviceScope.ServiceProvider.GetRequiredService<DbMasterData>();
        await dbMasterData.PopulateUserSettings();
    }

    /// <summary>
    ///     There is only one web server that gets started for each test class.<br />
    ///     This means OverrideServices is only called once before the first test run
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="builder"></param>
    protected virtual void OverrideServices(HostBuilderContext ctx, ContainerBuilder builder)
    {
    }

    protected async Task<SimpleTradingClient> CreateClient(bool includeAccessToken = true)
    {
        var client = factory.CreateClient();

        // ReSharper disable once InvertIf
        if (includeAccessToken)
        {
            var accessToken = await TestIdentity.AccessToken;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return new SimpleTradingClient(client);
    }

    protected async Task<T?> DbContextSingleOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
    {
        return await DbContext.Set<T>()
            .AsNoTracking()
            .SingleOrDefaultAsync(predicate);
    }
}