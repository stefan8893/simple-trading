using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Analyzers;

public class InteractorImplementorContext(
    INamedTypeSymbol interactor,
    INamedTypeSymbol requestModel,
    INamedTypeSymbol responseModel)
{
    public INamedTypeSymbol Interactor { get; } = interactor;
    public INamedTypeSymbol RequestModel { get; } = requestModel;
    public INamedTypeSymbol ResponseModel { get; } = responseModel;

    public bool IsResponseModelOneOf => ResponseModel.Name == "OneOf";

    public bool HasResponseModelOneOfCase(string oneOfCase)
    {
        return ResponseModel
            .TypeArguments
            .Any(x => x.Name == oneOfCase);
    }
}