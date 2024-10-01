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

            ReportDiagnostics(ctx, concreteInteractor);

            var interactorCtx = GatherInteractorContext(concreteInteractor);

            ctx.AddSource($"{interactorCtx.InteractorName}.g.cs",
                SourceText.From(SourceTemplates.CreateProxy(interactorCtx), Encoding.UTF8));
        });
    }

    private static void ReportDiagnostics(SourceProductionContext ctx, INamedTypeSymbol concreteInteractor)
    {
        if (concreteInteractor.Name.EndsWith("Interactor"))
            return;

        var missingInteractorSuffixDiagnostic = Diagnostic.Create(
            Rules.MissingInteractorSuffix,
            concreteInteractor.Locations.FirstOrDefault(),
            concreteInteractor.MetadataName);

        ctx.ReportDiagnostic(missingInteractorSuffixDiagnostic);
    }

    private static InteractorContext GatherInteractorContext(INamedTypeSymbol concreteInteractor)
    {
        var closedInteractorInterface = concreteInteractor
            .AllInterfaces
            .SingleOrDefault(static x => IsInteractorInterface(x));

        if (closedInteractorInterface is null)
            throw new Exception($"{concreteInteractor.MetadataName} does not implement 'IInteractor'");

        var genericTypeArguments = closedInteractorInterface.TypeArguments;
        var (requestModel, responseModel) = genericTypeArguments.Length == 1
            ? (null, (INamedTypeSymbol) genericTypeArguments[0])
            : ((INamedTypeSymbol) genericTypeArguments[0], (INamedTypeSymbol) genericTypeArguments[1]);

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