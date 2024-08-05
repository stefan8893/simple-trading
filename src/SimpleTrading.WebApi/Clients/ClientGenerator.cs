using System.Text;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;
using Swashbuckle.AspNetCore.Swagger;

namespace SimpleTrading.WebApi.Clients;

public class ClientGenerator(ILogger<ClientGenerator> logger, ISwaggerProvider swaggerProvider)
{
    public async Task Generate(Target target, DirectoryInfo[] outputDirectories, string fileName)
    {
        logger.LogInformation("Generate {target} Client", target);

        var swaggerDocument = swaggerProvider.GetSwagger("v1", null, "/");
        var swaggerFile = swaggerDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
        var openApiDocument = await OpenApiDocument.FromJsonAsync(swaggerFile);

        var fileContent = target switch
        {
            Target.CSharp => GenerateCSharpClient(openApiDocument),
            Target.TypeScript => GenerateTypeScriptClient(openApiDocument),
            _ => throw new ArgumentOutOfRangeException(nameof(Target))
        };

        foreach (var outputDirectory in outputDirectories)
        {
            var fullFileName = Path.Combine(outputDirectory.FullName, fileName);
            await File.WriteAllTextAsync(fullFileName, fileContent, Encoding.UTF8);

            logger.LogInformation("'{clientName}' created in '{directory}'", fileName, outputDirectory.FullName);
        }
    }

    private static string GenerateTypeScriptClient(OpenApiDocument openApiDocument)
    {
        return new TypeScriptClientGenerator(openApiDocument, ClientSettings.TypeScript).GenerateFile();
    }

    private static string GenerateCSharpClient(OpenApiDocument openApiDocument)
    {
        return new CSharpClientGenerator(openApiDocument, ClientSettings.CSharp).GenerateFile();
    }
}