namespace SimpleTrading.Domain.Generators;

public class InteractorProxySourceTemplate(InteractorContext context)
{
    private static readonly IReadOnlyList<string> DefaultNamespaces =
    [
        "System",
        "System.Threading.Tasks",
        "SimpleTrading.Domain.Infrastructure"
    ];

    private IEnumerable<string> Namespaces =>
    [
        .. DefaultNamespaces,
        OnValidation("FluentValidation")
    ];

    private IEnumerable<string> UsingStatements => Namespaces
        .Distinct()
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Select(static x => $"using {x};");

    private string RequestModelFullQualifiedTypeName => context.RequestModel is null
        ? string.Empty
        : context.RequestModel.ToDisplayString();

    private string RequestModelParameter => string.IsNullOrWhiteSpace(RequestModelFullQualifiedTypeName)
        ? string.Empty
        : $"{RequestModelFullQualifiedTypeName} requestModel";

    public string GenerateConcreteProxy()
    {
        var validatorsType = $"IEnumerable<IValidator<{RequestModelFullQualifiedTypeName}>>";
        var requestModelParameter = RequestModelParameter;

        // lang=C#
        return $$"""
                 //----------------------
                 // <auto-generated>
                 //     Generated using Source Generators
                 // </auto-generated>
                 //----------------------

                 {{string.Join("\n\r", UsingStatements)}}

                 namespace {{context.Interactor.ContainingNamespace.ToDisplayString()}};

                 public interface {{context.InteractorInterfaceName}}
                 {
                     Task<{{context.ResponseModel.ToDisplayString()}}> Execute({{OnContainsRequestModel(requestModelParameter)}});
                 }

                 public sealed class {{context.InteractorProxyName}} : {{context.InteractorInterfaceName}}
                 {
                     private readonly {{context.ClosedInteractorInterface.ToDisplayString()}}  _interactor;
                     {{OnValidation($"private readonly {validatorsType} _validators;")}}
                     
                     public {{context.InteractorProxyName}}(
                              {{context.ClosedInteractorInterface.ToDisplayString()}} interactor{{OnValidation(",")}}
                              {{OnValidation($"{validatorsType} validators")}})
                     {
                         _interactor = interactor;
                         {{OnValidation("_validators = validators;")}}
                     }
  
                     public async Task<{{context.ResponseModel.ToDisplayString()}}> Execute({{OnContainsRequestModel(requestModelParameter)}}) 
                     {
                            {{OnValidation(
                                // lang=C#
                                """
                                foreach(var validator in _validators) 
                                            {
                                                var validationResult = await validator.ValidateAsync(requestModel);
                                                if (!validationResult.IsValid)
                                                    return new BadInput(validationResult);
                                            }
                                """)}}
                 
                         return await {{OnContainsRequestModel("_interactor.Execute(requestModel);", "_interactor.Execute();")}}
                     }
                 }
                 """;
    }

    private string OnContainsRequestModel(string onRequestModelExists, string otherwise = "")
    {
        return context.RequestModel is not null ? onRequestModelExists : otherwise;
    }

    private string OnValidation(string onValidation, string otherwise = "")
    {
        var validateRequestModel = context.RequestModel is not null &&
                                   context.ResponseModel.IsGenericType &&
                                   context.ResponseModel.TypeArguments.Any(x => x.Name == "BadInput");

        return validateRequestModel ? onValidation : otherwise;
    }
}