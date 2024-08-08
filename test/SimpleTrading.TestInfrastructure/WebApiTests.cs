using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.TestInfrastructure.Authentication;
using SimpleTrading.WebApi;
using Xunit;

namespace SimpleTrading.TestInfrastructure;

public abstract class WebApiTests(TestingWebApplicationFactory<Program> factory)
    : TestBase, IClassFixture<TestingWebApplicationFactory<Program>>, IAsyncLifetime
{
    private TradingDbContext? _dbContext;
    private IServiceScope? _serviceScope;
    protected readonly WebApplicationFactory<Program> Factory = factory;

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
        _serviceScope = Factory.Services.CreateScope();
        _dbContext = _serviceScope.ServiceProvider.GetRequiredService<TradingDbContext>();

        await DbContext.Database.MigrateAsync();

        var dbMasterData = _serviceScope.ServiceProvider.GetRequiredService<DbMasterData>();
        await dbMasterData.PopulateUserSettings();
    }

    protected virtual void OverrideServices(WebHostBuilderContext ctx, IServiceCollection services)
    {
    }

    protected async Task<HttpClient> CreateClientWithAccessToken()
    {
        var accessToken = await TestIdentity.AccessToken;
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return client;
    }
}