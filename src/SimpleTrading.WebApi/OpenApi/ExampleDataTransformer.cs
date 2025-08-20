using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SimpleTrading.WebApi.Features.Trading.Dto;

namespace SimpleTrading.WebApi.OpenApi;

public class ExampleDataTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        // TODO: replace OpenApiObject with JsonObject when .NET 10 is released
        
        if (context.JsonTypeInfo.Type == typeof(AddTradeDto))
            schema.Example = new OpenApiObject
            {
                ["assetId"] = new OpenApiString("0c275c78-0508-4836-81d5-342e2445d60c"),
                ["profileId"] = new OpenApiString("401c519b-956a-4a5f-bd84-77e716817771"),
                ["currencyId"] = new OpenApiString("dd1f1281-7ec9-450e-8dd8-da1f4eb78629"),
                ["opened"] = new OpenApiString("2024-08-03T10:00:00+02:00"),
                ["size"] = new OpenApiInteger(5000),
                ["entryPrice"] = new OpenApiDouble(1.0),
            };

        return Task.CompletedTask;
    }
}