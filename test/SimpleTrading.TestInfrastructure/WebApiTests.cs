using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.TestInfrastructure.Authentication;
using SimpleTrading.WebApi;
using System.Net.Http.Headers;
using Xunit;

namespace SimpleTrading.TestInfrastructure;

public abstract class WebApiTests(TestingWebApplicationFactory<Program> factory)
    : TestBase, IClassFixture<TestingWebApplicationFactory<Program>>, IAsyncLifetime
{
    private IServiceScope? _serviceScope;
    private TradingDbContext? _dbContext;

    protected TradingDbContext DbContext => _dbContext!;
    protected WebApplicationFactory<Program> Factory = factory;

    protected IServiceProvider ServiceLocator => _serviceScope!.ServiceProvider;

    protected virtual void OverrideServices(WebHostBuilderContext ctx, IServiceCollection services) { }

    public async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        _serviceScope?.Dispose();
    }

    public async Task InitializeAsync()
    {
        factory.OverrideServices = OverrideServices;
        _serviceScope = Factory.Services.CreateScope();
        _dbContext = _serviceScope.ServiceProvider.GetRequiredService<TradingDbContext>();

        await DbContext.Database.EnsureCreatedAsync();

        var dbMasterData = _serviceScope.ServiceProvider.GetRequiredService<DbMasterData>();
        await dbMasterData.Seed();
    }

    public async Task<HttpClient> CreateClientWithAccessToken()
    {
        var accessToken = await TestIdentity.AccessToken;
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return client;
    }
}