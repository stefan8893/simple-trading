using JetBrains.Annotations;

namespace SimpleTrading.WebApi.Configuration;

[UsedImplicitly]
public record Scope(string Value, string Description);

public class ClientAppEntraIdConfig
{
    public required string AuthorizationUrl { init; get; }
    public required string TokenUrl { init; get; }
    public required string ClientId { init; get; }
    public required string RedirectUrl { init; get; }
    public required IReadOnlyList<Scope> Scopes { init; get; }
}