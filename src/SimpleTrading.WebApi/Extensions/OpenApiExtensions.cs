using Microsoft.OpenApi.Models;
using SimpleTrading.WebApi.Configuration;

namespace SimpleTrading.WebApi.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection ConfigureOpenApiDocumentation(this IServiceCollection services,
        ClientAppEntraIdConfig clientAppEntraIdConfig)
    {
        services
            .AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Entra ID", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(clientAppEntraIdConfig.AuthorizationUrl),
                            TokenUrl = new Uri(clientAppEntraIdConfig.TokenUrl),
                            Scopes = clientAppEntraIdConfig.Scopes.ToDictionary(x => x.Value, x => x.Description)
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Entra ID"}
                        },
                        clientAppEntraIdConfig.Scopes.Select(x => x.Value).ToArray()
                    }
                });
            });

        return services;
    }

    public static IApplicationBuilder ConfigureSwaggerUi(this IApplicationBuilder app,
        ClientAppEntraIdConfig clientAppEntraIdConfig)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.RoutePrefix = "";
            c.DocumentTitle = "Simple Trading - Web Api";
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleTrading - WebApi");
            c.OAuthClientId(clientAppEntraIdConfig.ClientId);
            c.OAuthUsePkce();
            c.OAuthScopeSeparator(" ");
            c.OAuth2RedirectUrl(clientAppEntraIdConfig.RedirectUrl);
        });

        return app;
    }
}