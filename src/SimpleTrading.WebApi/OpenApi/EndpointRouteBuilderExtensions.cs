using Scalar.AspNetCore;
using SimpleTrading.WebApi.Configuration;

namespace SimpleTrading.WebApi.OpenApi;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseScalarUi(this IEndpointRouteBuilder app,
        ClientAppEntraIdConfig clientAppEntraIdConfig)
    {
        app.MapScalarApiReference(string.Empty, options =>
            {
                options
                    .AddPreferredSecuritySchemes("OAuth2")
                    .AddAuthorizationCodeFlow("OAuth2", c =>
                    {
                        c.AuthorizationUrl = clientAppEntraIdConfig.AuthorizationUrl;
                        c.TokenUrl = clientAppEntraIdConfig.TokenUrl;
                        c.Pkce = Pkce.Sha256;
                        c.ClientId = clientAppEntraIdConfig.ClientId;
                        c.SelectedScopes = clientAppEntraIdConfig.Scopes.Select(x => x.Value);
                    });

                options.WithDefaultHttpClient(ScalarTarget.Http, ScalarClient.Http11);
            });

        return app;
    }
}