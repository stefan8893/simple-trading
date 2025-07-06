using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using SimpleTrading.WebApi.Configuration;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Extensions;

public static class OpenApiServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOpenApiDocumentation(this IServiceCollection services,
        ClientAppEntraIdConfig clientAppEntraIdConfig)
    {
        services
            .AddSwaggerGen(options =>
            {
                options.SupportNonNullableReferenceTypes();
                options.AddSecurityDefinition("Entra ID", new OpenApiSecurityScheme
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

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Entra ID"}
                        },
                        clientAppEntraIdConfig.Scopes.Select(x => x.Value).ToArray()
                    }
                });

                options.OrderActionsBy(api =>
                {
                    if (api.ActionDescriptor is not ControllerActionDescriptor descriptor) return string.Empty;

                    var orderAttribute = descriptor
                        .EndpointMetadata.OfType<SwaggerUiControllerPositionAttribute>()
                        .FirstOrDefault();

                    return orderAttribute is null
                        ? descriptor.ControllerName
                        : orderAttribute.Position.ToString();
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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
            c.InjectStylesheet("/swagger-ui-customization.css");
        });

        return app;
    }
}