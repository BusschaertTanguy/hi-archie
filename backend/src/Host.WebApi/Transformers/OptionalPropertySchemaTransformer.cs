using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Host.WebApi.Transformers;

public class OptionalPropertySchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        MakePropertiesOptional(schema);
        return Task.CompletedTask;
    }

    private static void MakePropertiesOptional(OpenApiSchema schema)
    {
        foreach (var property in schema.Properties)
        {
            if (property.Value.Nullable)
            {
                schema.Required.Remove(property.Key);
            }

            MakePropertiesOptional(property.Value);
        }
    }
}