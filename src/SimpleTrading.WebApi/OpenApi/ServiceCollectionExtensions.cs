using Microsoft.OpenApi.Models;
using SimpleTrading.WebApi.Configuration;

namespace SimpleTrading.WebApi.OpenApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services,
        ClientAppEntraIdConfig clientAppEntraIdConfig)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo()
                {
                    Title = "Simple Trading - Web Api",
                    Version = "v1",
                    Description = "Api for the simple trading application."
                };
                
                return Task.CompletedTask;
            });

            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        return services;
    }
}