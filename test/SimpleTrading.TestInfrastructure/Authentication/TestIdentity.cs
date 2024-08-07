using Microsoft.Extensions.Configuration;

namespace SimpleTrading.TestInfrastructure.Authentication;

public static class TestIdentity
{
    private static readonly Lazy<Task<string>> AccessTokenValue = new(LoadAccessTokenAsync);
    public static Task<string> AccessToken => AccessTokenValue.Value;

    private static async Task<string> LoadAccessTokenAsync()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("testsettings.json")
            .AddUserSecrets<ClientCredentialsFlow>()
            .AddEnvironmentVariables()
            .Build();

        var testIdentity = config
                               .GetSection("TestIdentity")
                               .Get<ClientCredentialsFlowConfiguration>()
                           ?? throw new Exception("Missing Test Identity");

        var tokenClient = new ClientCredentialsFlow();

        var tokenResponse = await tokenClient.AcquireTokenAsync(testIdentity);
        return tokenResponse.AccessToken ?? throw new Exception("Error while loading access token");
    }
}