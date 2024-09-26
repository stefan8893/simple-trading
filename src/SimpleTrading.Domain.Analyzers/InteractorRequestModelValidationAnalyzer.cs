using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SimpleTrading.Domain.Analyzers.SymbolFinder;

namespace SimpleTrading.Domain.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class InteractorRequestModelValidationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rules.MissingBadInputCase, Rules.ResponseModelTypeIsNotOneOf);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.EnableConcurrentExecution();

        context.RegisterCompilationAction(Analyze);
    }

    private static void Analyze(CompilationAnalysisContext context)
    {
        var interactorInterface = FindInteractorInterface(context);
        if (interactorInterface == null)
            return;

        var validatorByRequestModelName = FindAbstractValidators(context);

        if (context.CancellationToken.IsCancellationRequested)
            return;

        var interactorImplementors = GetInteractorImplementors(context, interactorInterface);

        var validatableInteractors = interactorImplementors
            .Where(x => validatorByRequestModelName.ContainsKey(x.RequestModel.Name))
            .ToList();

        var missingBadInputCaseDiagnostics = CreateMissingBadInputCaseDiagnostics(validatableInteractors,
            validatorByRequestModelName);

        var responseModelTypeIsNotOneOfDiagnostics = CreateResponseModelTypeIsNotOneOfDiagnostics(
            validatableInteractors,
            validatorByRequestModelName);

        if (context.CancellationToken.IsCancellationRequested)
            return;

        foreach (var diagnostic in missingBadInputCaseDiagnostics.Concat(responseModelTypeIsNotOneOfDiagnostics))
            context.ReportDiagnostic(diagnostic);
    }

    private static List<InteractorImplementor> GetInteractorImplementors(CompilationAnalysisContext context,
        INamedTypeSymbol interactorInterface)
    {
        var implementsInteractorFinder =
            new ImplementsInterfaceSymbolFinder(context.CancellationToken, interactorInterface);
        implementsInteractorFinder.FindIn(context.Compilation.GlobalNamespace);

        var interactorImplementors =
            implementsInteractorFinder
                .Result
                .Select(AddRequestAndResponseModelSymbols)
                .OfType<InteractorImplementor>()
                .ToList();
        return interactorImplementors;
    }

    private static Dictionary<string, List<INamedTypeSymbol>> FindAbstractValidators(CompilationAnalysisContext context)
    {
        var abstractValidatorsFinder = new AbstractValidatorSymbolFinder(context.CancellationToken);
        abstractValidatorsFinder.FindIn(context.Compilation.GlobalNamespace);

        return abstractValidatorsFinder
            .Result
            .Where(x => x.BaseType is not null && x.BaseType.IsGenericType && x.BaseType.Arity is 1)
            .GroupBy(x => x.BaseType!.TypeArguments[0].Name)
            .ToDictionary(x => x.Key, x => x.ToList());
    }

    private static INamedTypeSymbol? FindInteractorInterface(CompilationAnalysisContext context)
    {
        return context.Compilation
            .GetSymbolsWithName(x => x == "IInteractor", SymbolFilter.Type)
            .OfType<INamedTypeSymbol>()
            .SingleOrDefault(x => x.MetadataName == "IInteractor`2" && x is {IsGenericType: true, Arity: 2});
    }

    private static IEnumerable<Diagnostic> CreateResponseModelTypeIsNotOneOfDiagnostics(
        List<InteractorImplementor> validatableInteractors,
        Dictionary<string, List<INamedTypeSymbol>> abstractValidatorByRequestModelName)
    {
        var validatableInteractorsWithoutOneOfResponseModel = validatableInteractors
            .Where(x => !x.IsResponseModelOneOf)
            .Select(x =>
                new BadInteractorDiagnosticContext(x, abstractValidatorByRequestModelName[x.RequestModel.Name]))
            .ToList();

        return validatableInteractorsWithoutOneOfResponseModel
            .Where(x => x.Interactor.Locations.Any())
            .Select(badInteractor => Diagnostic.Create(
                Rules.ResponseModelTypeIsNotOneOf,
                badInteractor.Interactor.Locations.First(),
                badInteractor.RequestModel.Name)
            );
    }

    private static IEnumerable<Diagnostic> CreateMissingBadInputCaseDiagnostics(
        List<InteractorImplementor> validatableInteractors,
        Dictionary<string, List<INamedTypeSymbol>> abstractValidatorByRequestModelName)
    {
        var validatableInteractorsWithoutBadInputCase = validatableInteractors
            .Where(x => x.IsResponseModelOneOf)
            .Where(x => !x.HasResponseModelOneOfCase("BadInput"))
            .Select(x =>
                new BadInteractorDiagnosticContext(x, abstractValidatorByRequestModelName[x.RequestModel.Name]))
            .ToList();

        return validatableInteractorsWithoutBadInputCase
            .Where(x => x.Interactor.Locations.Any())
            .Select(badInteractor => Diagnostic.Create(
                Rules.MissingBadInputCase,
                badInteractor.Interactor.Locations.First(),
                badInteractor.RequestModel.Name)
            );
    }

    private static InteractorImplementor? AddRequestAndResponseModelSymbols(
        INamedTypeSymbol interactorImplementor)
    {
        var symbol = interactorImplementor
            .AllInterfaces
            .FirstOrDefault(i => i.MetadataName == "IInteractor`2");

        if (symbol is null)
            return null;

        var typeArguments = symbol.TypeArguments;
        if (typeArguments.Length != 2)
            return null;

        return new InteractorImplementor(interactorImplementor,
            (INamedTypeSymbol) typeArguments[0],
            (INamedTypeSymbol) typeArguments[1]);
    }
}