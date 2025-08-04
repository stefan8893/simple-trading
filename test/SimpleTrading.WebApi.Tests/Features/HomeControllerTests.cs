using SimpleTrading.TestInfrastructure;

namespace SimpleTrading.WebApi.Tests.Features;

public class HomeControllerTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task InfoEndpoint_returns_app_info()
    {
        var client = await CreateClient(false);

        var response = await client.GetAppInfoAsync(TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        Assert.NotNull(response.Name);
        Assert.NotNull(response.Environment);
        Assert.NotNull(response.Version);
    }
}