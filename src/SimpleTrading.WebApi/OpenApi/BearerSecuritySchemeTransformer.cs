using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace SimpleTrading.WebApi.OpenApi;

public class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == JwtBearerDefaults.AuthenticationScheme))
        {
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["OAuth2"] = new()
                {
                    Type = SecuritySchemeType.OAuth2,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token",
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;
        }

        // Apply it as a requirement for all operations
        foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
            operation.Value.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme {Reference = new OpenApiReference {Id = "Bearer", Type = ReferenceType.SecurityScheme}}] =
                    Array.Empty<string>()
            });
    }
}