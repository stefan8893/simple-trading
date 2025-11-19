using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using SimpleTrading.WebApi.Features.Trading.Dto;

namespace SimpleTrading.WebApi.OpenApi;

public class ExampleDataTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type == typeof(AddTradeDto))
            schema.Example = new JsonObject
            {
                ["assetId"] = "0c275c78-0508-4836-81d5-342e2445d60c",
                ["profileId"] = "401c519b-956a-4a5f-bd84-77e716817771",
                ["currencyId"] = "dd1f1281-7ec9-450e-8dd8-da1f4eb78629",
                ["opened"] = DateTime.Parse("2024-08-03T10:00:00+02:00"),
                ["size"] = 5000,
                ["entryPrice"] = 1.0,
            };

        return Task.CompletedTask;
    }
}