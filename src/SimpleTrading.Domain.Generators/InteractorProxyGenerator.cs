using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SimpleTrading.Domain.Generators;

[Generator(LanguageNames.CSharp)]
public class InteractorProxyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        AddInfrastructureSourceCode(context);

        var interactors = CollectInteractors(context);
        var validators = CollectValidators(context);
        var validationResult = CollectValidationResult(context);

        var combinedValueProvider = interactors
            .Combine(validators)
            .Combine(validationResult)
            .Select((triple, _) =>
            {
                var (pair, validationResultCollected) = triple;
                var (interactorsCollected, validatorsCollected) = pair;
                return (interactorsCollected, validatorsCollected, validationResultCollected);
            });

        GenerateProxyInfrastructure(context, combinedValueProvider);
    }

    private static void AddInfrastructureSourceCode(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx
            => ctx.AddSource("InteractorProxyValidationResult.g.cs",
                InfrastructureSource.ValidationResultMarkerAttribute));

        context.RegisterPostInitializationOutput(ctx
            => ctx.AddSource("IInteractor.cs", InfrastructureSource.InteractorInterface));
    }

    private static IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> CollectValidationResult(
        IncrementalGeneratorInitializationContext context)
    {
        return context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                "SimpleTrading.Domain.Infrastructure.InteractorProxyValidationResultAttribute",
                static (node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
                static (ctx, _) => (INamedTypeSymbol) ctx.TargetSymbol)
            .Collect();
    }

    private static IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> CollectValidators(
        IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax {BaseList: not null},
                static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node) as INamedTypeSymbol)
            .Where(static x => x is not null && !x.IsAbstract)
            .Where(static x => IsValidator(x!))
            .Select(static (symbol, _) => symbol!)
            .Collect();
    }

    private static IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> CollectInteractors(
        IncrementalGeneratorInitializationContext context)
    {
        return context
            .SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => s is ClassDeclarationSyntax {BaseList: not null},
                static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node) as INamedTypeSymbol
            )
            .Where(static x => x is not null && !x.IsAbstract)
            .Where(static x => ImplementsInteractor(x!))
            .Select(static (symbol, _) => symbol!)
            .Collect();
    }

    private static void GenerateProxyInfrastructure(IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<(ImmutableArray<INamedTypeSymbol> Interactors, ImmutableArray<INamedTypeSymbol>
            Validators, ImmutableArray<INamedTypeSymbol> ValidationResult)> combined)
    {
        context.RegisterSourceOutput(combined, static (ctx, combined) =>
        {
            var (interactors,
                validators,
                validationResult) = combined;

            if (validationResult.IsEmpty)
                // TODO: Report Diagnostic -> No validation result type provided (e.g., BadInput)
                return;

            if (validationResult.Length > 1)
                // TODO: Report Diagnostic -> Multiple validation result types provided (e.g. BadInput and ValidationFailed)
                return;

            // TODO: verify validation result has a constructor that takes a FluentValidation.ValidationResult
            // if (...)
            // {
            //     // TODO: Report Diagnostic -> InteractorProxyValidationResult needs a constructor that takes a FluentValidation.ValidationResult
            //     return;
            // }

            var interactorContexts = CombineToInteractorContexts(interactors, validators, validationResult[0]);

            foreach (var interactorCtx in interactorContexts)
                GenerateProxy(interactorCtx, ctx);
        });
    }

    private static void GenerateProxy(InteractorContext interactorContext, SourceProductionContext ctx)
    {
        var interactorProxySourceTemplate = new InteractorProxySourceTemplate(interactorContext);

        ctx.AddSource($"{interactorContext.InteractorName}.g.cs",
            SourceText.From(interactorProxySourceTemplate.GenerateProxy(), Encoding.UTF8));
    }

    private static ImmutableArray<InteractorContext> CombineToInteractorContexts(
        ImmutableArray<INamedTypeSymbol> interactors, ImmutableArray<INamedTypeSymbol> validators,
        INamedTypeSymbol validationResult)
    {
        var validatorsByValidatedType = validators
            .Where(x => x.BaseType!.TypeArguments.Length == 1)
            .GroupBy(v => v.BaseType!.TypeArguments[0], SymbolEqualityComparer.Default)
            .Where(x => x.Key is not null)
            .ToImmutableDictionary(
                key => key.Key!,
                value => value.Select(v => v).ToImmutableArray(),
                SymbolEqualityComparer.Default);

        return
        [
            ..interactors
                .Select(x => GatherInteractorContext(x, validatorsByValidatedType, validationResult))
                .SelectMany(x => x is not null ? [x] : Array.Empty<InteractorContext>())
        ];
    }

    private static InteractorContext? GatherInteractorContext(INamedTypeSymbol concreteInteractor,
        ImmutableDictionary<ISymbol, ImmutableArray<INamedTypeSymbol>> validatorsByValidatedType,
        INamedTypeSymbol validationResult)
    {
        var closedInteractorInterface = concreteInteractor
            .AllInterfaces
            .FirstOrDefault(static x => IsInteractorInterface(x));

        if (closedInteractorInterface is null)
            return null;

        var genericTypeArguments = closedInteractorInterface.TypeArguments
            .OfType<INamedTypeSymbol>()
            .ToImmutableArray();

        if (genericTypeArguments.Length is not (1 or 2))
            return null;

        var (requestModel, responseModel) = genericTypeArguments.Length == 1
            ? (null, genericTypeArguments[0])
            : (genericTypeArguments[0], genericTypeArguments[1]);

        var validators = requestModel is not null
            ? validatorsByValidatedType.GetValueOrDefault(requestModel, ImmutableArray<INamedTypeSymbol>.Empty)
            : ImmutableArray<INamedTypeSymbol>.Empty;

        return new InteractorContext(concreteInteractor,
            closedInteractorInterface,
            requestModel,
            responseModel,
            validators,
            validationResult);
    }

    private static bool IsValidator(INamedTypeSymbol candidate)
    {
        return candidate.BaseType is
        {
            IsGenericType: true,
            MetadataName: "AbstractValidator`1",
            Arity: 1,
            ContainingNamespace.MetadataName: "FluentValidation"
        };
    }

    private static bool ImplementsInteractor(INamedTypeSymbol candidate)
    {
        return candidate
            .AllInterfaces
            .Any(static i => IsInteractorInterface(i));
    }

    private static bool IsInteractorInterface(INamedTypeSymbol candidate)
    {
        return candidate is
        {
            Name: "IInteractor",
            IsGenericType: true,
            Arity: 1 or 2
        } && candidate
            .ContainingNamespace
            .ToDisplayString().Equals("SimpleTrading.Domain.Infrastructure", StringComparison.OrdinalIgnoreCase);
    }
}