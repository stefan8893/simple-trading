
using Microsoft.OpenApi;

namespace SimpleTrading.WebApi.OpenApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "SimpleTrading - Api",
                    Version = "v1",
                    Description = "Api for the simple trading application."
                };

                return Task.CompletedTask;
            });

            options.AddDocumentTransformer<OAuth2SecuritySchemeTransformer>();
            options.AddDocumentTransformer<BaseUrlTransformer>();
            options.AddSchemaTransformer<ExampleDataTransformer>();
        });

        return services;
    }
}