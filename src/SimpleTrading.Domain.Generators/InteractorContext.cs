using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Generators;

public class InteractorContext
{
    public readonly INamedTypeSymbol ClosedInteractorInterface;
    public readonly bool HasValidators;
    public readonly INamedTypeSymbol Interactor;
    public readonly string InteractorInterfaceName;
    public readonly string InteractorName;
    public readonly string InteractorProxyName;
    public readonly bool IsResponseModelOneOf;
    public readonly bool IsResponseModelOneOfWithValidationResultCase;
    public readonly INamedTypeSymbol? RequestModel;
    public readonly INamedTypeSymbol ResponseModel;
    public readonly ImmutableArray<INamedTypeSymbol> Validators;

    public InteractorContext(INamedTypeSymbol interactor,
        INamedTypeSymbol closedInteractorInterface,
        INamedTypeSymbol? requestModel,
        INamedTypeSymbol responseModel,
        ImmutableArray<INamedTypeSymbol> validators)
    {
        Interactor = interactor;
        ClosedInteractorInterface = closedInteractorInterface;
        RequestModel = requestModel;
        ResponseModel = responseModel;
        Validators = validators;

        InteractorName = interactor.Name.Replace("Interactor", "");
        InteractorInterfaceName = $"I{Interactor.Name.Replace("Interactor", "")}";
        InteractorProxyName = $"{Interactor.Name}Proxy";

        HasValidators = !validators.IsEmpty;
        IsResponseModelOneOf = responseModel is {IsGenericType: true, Name: "OneOf"};
        IsResponseModelOneOfWithValidationResultCase = IsResponseModelOneOf &&
                                                       responseModel.TypeArguments.Any(x =>
                                                           x.ToDisplayString()
                                                               .Equals("FluentValidation.Results.ValidationResult",
                                                                   StringComparison.Ordinal));
    }

    public (bool isTransformed, string responseModel) GetResponseModelTransformed()
    {
        var addValidationResultCase = HasValidators && !IsResponseModelOneOfWithValidationResultCase;
        return addValidationResultCase
            ? (true, AddValidationResultCaseToResponseModel())
            : (false, ResponseModel.GetDisplayName());
    }

    private string AddValidationResultCaseToResponseModel()
    {
        return !IsResponseModelOneOf
            ? ConvertResponseModelToOneOfWithValidationResultCase()
            : AddValidationResultCaseToExistingOneOfResponseModel();
    }

    private string ConvertResponseModelToOneOfWithValidationResultCase()
    {
        return $"OneOf<{ResponseModel.GetDisplayName()}, ValidationResult>";
    }

    private string AddValidationResultCaseToExistingOneOfResponseModel()
    {
        return $"{ResponseModel.GetDisplayName().TrimEnd('>')}, ValidationResult>";
    }
}