using FluentAssertions;
using SimpleTrading.TestInfrastructure;

namespace SimpleTrading.WebApi.Tests.Features;

public class HomeControllerTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task InfoEndpoint_returns_app_info()
    {
        var client = await CreateClient(includeAccessToken: false);

        var response = await client.GetAppInfoAsync();

        response.Should().NotBeNull();
        response.Name.Should().NotBeNull();
        response.Environment.Should().NotBeNull();
        response.Version.Should().NotBeNull();
    }
}