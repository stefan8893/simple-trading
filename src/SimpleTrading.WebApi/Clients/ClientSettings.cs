using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;

namespace SimpleTrading.WebApi.Clients;

public static class ClientSettings
{
    public static readonly CSharpClientGeneratorSettings CSharp = new()
    {
        ClassName = "SimpleTradingApiClient",
        CSharpGeneratorSettings =
        {
            Namespace = "SimpleTrading.ApiClient",
            GenerateOptionalPropertiesAsNullable = true
        },
        GenerateExceptionClasses = true,
        ExceptionClass = "SimpleTradingApiClientException",
        GenerateOptionalParameters = true,
        GenerateClientClasses = true,
        GenerateClientInterfaces = true,
        GenerateDtoTypes = true,
        InjectHttpClient = true,
        UseBaseUrl = false,
        WrapResponses = false,
        ResponseClass = "SimpleTradingApiClientResponse",
        WrapDtoExceptions = true
    };

    public static readonly TypeScriptClientGeneratorSettings TypeScript = new()
    {
        ClassName = "SimpleTradingApiClient",
        Template = TypeScriptTemplate.Fetch,
        ExceptionClass = "SimpleTradingApiClientException",
        PromiseType = PromiseType.Promise,
        WrapResponses = true,
        GenerateClientInterfaces = true,
        GenerateClientClasses = true,
        GenerateDtoTypes = true,
        ResponseClass = "SimpleTradingApiClientResponse",
        UseAbortSignal = true,
        TypeScriptGeneratorSettings =
        {
            EnumStyle = TypeScriptEnumStyle.StringLiteral
        }
    };
}