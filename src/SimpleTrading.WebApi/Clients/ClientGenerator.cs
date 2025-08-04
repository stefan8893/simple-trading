using System.Text;
using Microsoft.OpenApi;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;
using SimpleTrading.WebApi.CliCommands;

namespace SimpleTrading.WebApi.Clients;

public class ClientGenerator(ILogger<ClientGenerator> logger)
{
    private const string ApiDescriptionFileName = "simple-trading-api-description.json";
    
    public async Task Generate(Target target, DirectoryInfo[] outputDirectories, string fileName)
    {
        logger.LogInformation("Generate {target} Client", target);
        
        var apiDescriptionFile = Path.Combine(AppContext.BaseDirectory, ApiDescriptionFileName);
        var openDescription = await File.ReadAllTextAsync(apiDescriptionFile);
        var openApiDocument = await OpenApiDocument.FromJsonAsync(openDescription);
        
        var fileContent = target switch
        {
            Target.CSharp => GenerateCSharpClient(openApiDocument),
            Target.TypeScript => GenerateTypeScriptClient(openApiDocument),
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
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