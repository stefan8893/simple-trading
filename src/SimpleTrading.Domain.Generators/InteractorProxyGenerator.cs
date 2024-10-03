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
        var interactors = context
            .SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => s is ClassDeclarationSyntax,
                static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node) as INamedTypeSymbol
            )
            .Where(static x => x is not null && !x.IsAbstract)
            .Where(static x => ImplementsInteractor(x!));

        context.RegisterSourceOutput(interactors, (ctx, concreteInteractor) =>
        {
            if (concreteInteractor is null)
                return;

            var interactorCtx = GatherInteractorContext(concreteInteractor);
            if (interactorCtx is null)
                return;

            ctx.AddSource($"{interactorCtx.InteractorName}.g.cs",
                SourceText.From(SourceTemplates.CreateProxy(interactorCtx), Encoding.UTF8));
        });
    }

    private static InteractorContext? GatherInteractorContext(INamedTypeSymbol concreteInteractor)
    {
        var closedInteractorInterface = concreteInteractor
            .AllInterfaces
            .SingleOrDefault(static x => IsInteractorInterface(x));

        if (closedInteractorInterface is null)
            throw new Exception($"{concreteInteractor.MetadataName} does not implement 'IInteractor'");

        var genericTypeArguments = closedInteractorInterface.TypeArguments
            .OfType<INamedTypeSymbol>()
            .ToImmutableArray();

        if (genericTypeArguments.Length is not (1 or 2))
            return null;

        var (requestModel, responseModel) = genericTypeArguments.Length == 1
            ? (null, genericTypeArguments[0])
            : (genericTypeArguments[0], genericTypeArguments[1]);

        return new InteractorContext(concreteInteractor, closedInteractorInterface, responseModel)
        {
            RequestModel = requestModel
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
            .ToDisplayString() == "SimpleTrading.Domain.Abstractions";
    }
}