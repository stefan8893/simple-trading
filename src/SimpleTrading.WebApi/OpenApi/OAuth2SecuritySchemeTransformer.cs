using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using SimpleTrading.WebApi.Configuration;

namespace SimpleTrading.WebApi.OpenApi;

public class OAuth2SecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider,
    ClientAppEntraIdConfig clientAppEntraIdConfig)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == JwtBearerDefaults.AuthenticationScheme))
        {
            var requirements = new Dictionary<string, IOpenApiSecurityScheme>
            {
                [JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token",
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(clientAppEntraIdConfig.AuthorizationUrl),
                            TokenUrl = new Uri(clientAppEntraIdConfig.TokenUrl),
                            Scopes = clientAppEntraIdConfig.Scopes.ToDictionary(k => k.Value, v => v.Description)
                        }
                    }
                }
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;
        }
    }
}