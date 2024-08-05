using FluentAssertions;
using SimpleTrading.Client;
using SimpleTrading.TestInfrastructure;

namespace SimpleTrading.WebApi.Tests.Features;

public class HomeControllerTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task InfoEndpoint_returns_app_info()
    {
        var client = Factory.CreateClient();
        var simpleTradingClient = new SimpleTradingClient(client);

        var response = await simpleTradingClient.GetAppInfoAsync();

        response.Should().NotBeNull();
        response.Name.Should().NotBeNull();
        response.Environment.Should().NotBeNull();
        response.Version.Should().NotBeNull();
    }
}