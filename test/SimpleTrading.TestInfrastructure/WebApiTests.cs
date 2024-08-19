using System.Linq.Expressions;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Client;
using SimpleTrading.DataAccess;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.TestInfrastructure.Authentication;
using SimpleTrading.WebApi;
using Xunit;

namespace SimpleTrading.TestInfrastructure;

public abstract class WebApiTests(TestingWebApplicationFactory<Program> factory)
    : TestBase, IClassFixture<TestingWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory = factory;
    private TradingDbContext? _dbContext;
    private IServiceScope? _serviceScope;

    protected TradingDbContext DbContext => _dbContext!;

    protected IServiceProvider ServiceLocator => _serviceScope!.ServiceProvider;

    public async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        _serviceScope?.Dispose();
    }

    public async Task InitializeAsync()
    {
        factory.OverrideServices = OverrideServices;
        _serviceScope = _factory.Services.CreateScope();
        _dbContext = _serviceScope.ServiceProvider.GetRequiredService<TradingDbContext>();

        await DbContext.Database.MigrateAsync();

        var dbMasterData = _serviceScope.ServiceProvider.GetRequiredService<DbMasterData>();
        await dbMasterData.PopulateUserSettings();
    }

    protected virtual void OverrideServices(WebHostBuilderContext ctx, IServiceCollection services)
    {
    }

    protected async Task<SimpleTradingClient> CreateClient(bool includeAccessToken = true)
    {
        var client = _factory.CreateClient();

        if (!includeAccessToken)
            return new SimpleTradingClient(client);

        var accessToken = await TestIdentity.AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return new SimpleTradingClient(client);
    }

    protected async Task<T?> DbContextSingleOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
    {
        return await DbContext.Set<T>()
            .AsNoTracking()
            .SingleOrDefaultAsync(predicate);
    }
}