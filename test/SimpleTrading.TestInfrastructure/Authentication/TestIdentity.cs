using Microsoft.Extensions.Configuration;

namespace SimpleTrading.TestInfrastructure.Authentication;

public static class TestIdentity
{
    private static readonly Lazy<Task<string>> AccessTokenValue = new(LoadAccessTokenAsync);
    public static Task<string> AccessToken => AccessTokenValue.Value;

    private static async Task<string> LoadAccessTokenAsync()
    {
        var testIdentity = GetTestIdentityConfiguration();

        var tokenClient = new ClientCredentialsFlow();
        var tokenResponse = await tokenClient.AcquireTokenAsync(testIdentity);

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