using Microsoft.Extensions.Configuration;

namespace SimpleTrading.TestInfrastructure.Authentication;

public static class TestIdentity
{
    private static readonly Lazy<Task<string>> AccessTokenValue = new(LoadAccessToken);
    public static Task<string> AccessToken => AccessTokenValue.Value;

    private static async Task<string> LoadAccessToken()
    {
        var testIdentity = GetTestIdentityConfiguration();

        var tokenClient = new ClientCredentialsFlow();
        var tokenResponse = await tokenClient.AcquireToken(testIdentity);

        return tokenResponse.AccessToken
               ?? throw new Exception("Error while loading access token");
    }

    private static ClientCredentialsFlowConfiguration GetTestIdentityConfiguration()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("testsettings.json")
            .AddUserSecrets<ClientCredentialsFlow>()
            .AddEnvironmentVariables()
            .Build();

        return config
                   .GetSection("TestIdentity")
                   .Get<ClientCredentialsFlowConfiguration>()
               ?? throw new Exception("Missing Test Identity");
    }
}