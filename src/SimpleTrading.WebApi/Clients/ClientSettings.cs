using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;

namespace SimpleTrading.WebApi.Clients;

public static class ClientSettings
{
    public static readonly CSharpClientGeneratorSettings CSharp = new()
    {
        ClassName = "SimpleTradingClient",
        CSharpGeneratorSettings =
        {
            Namespace = "SimpleTrading.Client",
            GenerateOptionalPropertiesAsNullable = true
        },
        GenerateExceptionClasses = true,
        ExceptionClass = "SimpleTradingClientException",
        GenerateOptionalParameters = true,
        GenerateClientClasses = true,
        GenerateClientInterfaces = true,
        GenerateDtoTypes = true,
        InjectHttpClient = true,
        WrapResponses = false,
        ResponseClass = "SimpleTradingClientResponse",
        WrapDtoExceptions = true
    };

    public static readonly TypeScriptClientGeneratorSettings TypeScript = new()
    {
        ClassName = "SimpleTradingClient",
        Template = TypeScriptTemplate.Fetch,
        ExceptionClass = "SimpleTradingClientException",
        PromiseType = PromiseType.Promise,
        WrapResponses = true,
        GenerateClientInterfaces = true,
        GenerateClientClasses = true,
        GenerateDtoTypes = true,
        WrapDtoExceptions = true
    };
}