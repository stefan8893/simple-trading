using JetBrains.Annotations;

namespace SimpleTrading.WebApi.Configuration;

[UsedImplicitly]
public record Scope(string Value, string Description);

public class ClientAppEntraIdConfig
{
    public required string AuthorizationUrl { get; init; }
    public required string TokenUrl { get; init; }
    public required string ClientId { get; init; }
    public required IReadOnlyList<Scope> Scopes { get; init; }
}