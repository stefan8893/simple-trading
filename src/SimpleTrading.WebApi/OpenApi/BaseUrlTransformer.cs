using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SimpleTrading.WebApi.OpenApi;

public class BaseUrlTransformer(IConfiguration configuration)
    : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var baseUrl = configuration.GetValue<string>("BaseUrl");

        if (baseUrl is null)
            return Task.CompletedTask;

        document.Servers?.Clear();
        document.Servers?.Add(new OpenApiServer {Url = baseUrl});

        return Task.CompletedTask;
    }
}