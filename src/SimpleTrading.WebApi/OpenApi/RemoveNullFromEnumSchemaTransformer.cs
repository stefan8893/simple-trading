using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SimpleTrading.WebApi.OpenApi;

public class RemoveNullFromEnumSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (schema.Enum is null)
            return Task.CompletedTask;

        schema.Enum = schema.Enum
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            .Where(x => x is not null)
            .ToList();

        return Task.CompletedTask;
    }
}