using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SimpleTrading.TestInfrastructure.Authentication;

public record ClientCredentialsFlowConfiguration
{
    public string? TokenEndpoint { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Scope { get; set; }
}

public record TokenResponse
{
    [JsonPropertyName("access_token")] public string? AccessToken { get; set; }

    [JsonPropertyName("token_type")] public string? TokenType { get; set; }

    [JsonPropertyName("expires_in")] public int? ExpiresInSeconds { get; set; }
}

public class ClientCredentialsFlow(HttpClient httpClient)
{
    public ClientCredentialsFlow()
        : this(new HttpClient())
    {
    }

    public async Task<TokenResponse> AcquireTokenAsync(ClientCredentialsFlowConfiguration config)
    {
        if (string.IsNullOrWhiteSpace(config.TokenEndpoint))
            throw new Exception($"Missing {nameof(config.TokenEndpoint)}");

        var tokenRequestMessage = new HttpRequestMessage(HttpMethod.Post, config.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = config.ClientId ?? throw new Exception($"Missing {nameof(config.ClientId)}"),
                ["client_secret"] =
                    config.ClientSecret ?? throw new Exception($"Missing {nameof(config.ClientSecret)}"),
                ["scope"] = config.Scope ?? throw new Exception($"Missing {nameof(config.Scope)}")
            })
        };

        var response = await httpClient.SendAsync(tokenRequestMessage);
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();

        return token ?? throw new Exception("Error while fetching token from Entra ID");
    }
}