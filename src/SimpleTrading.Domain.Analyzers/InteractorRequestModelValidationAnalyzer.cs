﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SimpleTrading.Domain.Analyzers.SymbolCollector;

namespace SimpleTrading.Domain.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class InteractorRequestModelValidationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rules.MissingBadInputCase,
            Rules.ResponseModelTypeMustBeOneOf,
            Rules.MissingInteractorSuffix);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.EnableConcurrentExecution();

        context.RegisterCompilationAction(Analyze);
    }

    private static void Analyze(CompilationAnalysisContext context)
    {
        var interactorInterface = FindInteractorInterface(context);
        if (interactorInterface is null)
            return;

        var validatorByRequestModelName = FindAbstractValidators(context)
            .GroupBy(x => x.BaseType!.TypeArguments[0].Name)
            .ToDictionary(x => x.Key, x => x.ToList());

        if (context.CancellationToken.IsCancellationRequested)
            return;

        var interactorImplementors = GetInteractorImplementors(context, interactorInterface);
        var diagnostics = GatherDiagnostics(validatorByRequestModelName, interactorImplementors);

        if (context.CancellationToken.IsCancellationRequested)
            return;

        foreach (var diagnostic in diagnostics)
            context.ReportDiagnostic(diagnostic);
    }

    private static INamedTypeSymbol? FindInteractorInterface(CompilationAnalysisContext context)
    {
        return context.Compilation
            .GetSymbolsWithName(x => x == "IInteractor", SymbolFilter.Type)
            .OfType<INamedTypeSymbol>()
            .SingleOrDefault(x => x.MetadataName == "IInteractor`2" && x is {IsGenericType: true, Arity: 2});
    }

    private static IEnumerable<INamedTypeSymbol> FindAbstractValidators(CompilationAnalysisContext context)
    {
        return new AbstractValidatorSymbolCollector(context.CancellationToken)
            .CollectIn(context.Compilation.GlobalNamespace);
    }

    private static List<InteractorImplementorContext> GetInteractorImplementors(CompilationAnalysisContext context,
        INamedTypeSymbol interactorInterface)
    {
        var implementsInteractorSymbolCollector =
            new ImplementsInterfaceSymbolCollector(context.CancellationToken, interactorInterface);

        return implementsInteractorSymbolCollector
            .CollectIn(context.Compilation.GlobalNamespace)
            .Select(AddRequestAndResponseModelSymbols)
            .OfType<InteractorImplementorContext>()
            .ToList();
    }

    private static InteractorImplementorContext? AddRequestAndResponseModelSymbols(
        INamedTypeSymbol interactorImplementor)
    {
        var symbol = interactorImplementor
            .AllInterfaces
            .FirstOrDefault(i => i.MetadataName == "IInteractor`2");

        if (symbol is null)
            return null;

        var typeArguments = symbol
            .TypeArguments
            .OfType<INamedTypeSymbol>()
            .ToImmutableArray();

        if (typeArguments.Length != 2)
            return null;

        return new InteractorImplementorContext(interactorImplementor,
            typeArguments[0],
            typeArguments[1]);
    }

    private static IEnumerable<Diagnostic> GatherDiagnostics(
        IReadOnlyDictionary<string, List<INamedTypeSymbol>> validatorByRequestModelName,
        List<InteractorImplementorContext> interactorImplementors)
    {
        var interactorsWithRequestModelValidators = interactorImplementors
            .Where(x => validatorByRequestModelName.ContainsKey(x.RequestModel.Name))
            .ToList();

        var missingBadInputCaseDiagnostics = DetectMissingBadInputCaseDiagnostics(
            interactorsWithRequestModelValidators,
            validatorByRequestModelName);

        var responseModelTypeIsNotOneOfDiagnostics = DetectResponseModelTypeIsNotOneOfDiagnostics(
            interactorsWithRequestModelValidators,
            validatorByRequestModelName);

        var missingInteractorSuffixDiagnostics = DetectMissingInteractorSuffixDiagnostics(interactorImplementors,
            validatorByRequestModelName);

        return missingBadInputCaseDiagnostics
            .Concat(responseModelTypeIsNotOneOfDiagnostics)
            .Concat(missingInteractorSuffixDiagnostics);
    }

    private static IEnumerable<Diagnostic> DetectResponseModelTypeIsNotOneOfDiagnostics(
        List<InteractorImplementorContext> validatableInteractors,
        IReadOnlyDictionary<string, List<INamedTypeSymbol>> abstractValidatorByRequestModelName)
    {
        var validatableInteractorsWithoutOneOfResponseModel = validatableInteractors
            .Where(x => !x.IsResponseModelOneOf)
            .Select(x =>
                new BadInteractorDiagnosticContext(x, abstractValidatorByRequestModelName[x.RequestModel.Name]));

        return validatableInteractorsWithoutOneOfResponseModel
            .Where(x => x.Interactor.Locations.Any())
            .Select(badInteractor => Diagnostic.Create(
                Rules.ResponseModelTypeMustBeOneOf,
                badInteractor.Interactor.Locations.First(),
                badInteractor.RequestModel.Name)
            );
    }

    private static IEnumerable<Diagnostic> DetectMissingBadInputCaseDiagnostics(
        List<InteractorImplementorContext> validatableInteractors,
        IReadOnlyDictionary<string, List<INamedTypeSymbol>> abstractValidatorByRequestModelName)
    {
        var validatableInteractorsWithoutBadInputCase = validatableInteractors
            .Where(x => x.IsResponseModelOneOf)
            .Where(x => !x.HasResponseModelOneOfCase("BadInput"))
            .Select(x =>
                new BadInteractorDiagnosticContext(x, abstractValidatorByRequestModelName[x.RequestModel.Name]));

        return validatableInteractorsWithoutBadInputCase
            .Where(x => x.Interactor.Locations.Any())
            .Select(badInteractor => Diagnostic.Create(
                Rules.MissingBadInputCase,
                badInteractor.Interactor.Locations.First(),
                badInteractor.RequestModel.Name)
            );
    }

    private static IEnumerable<Diagnostic> DetectMissingInteractorSuffixDiagnostics(
        List<InteractorImplementorContext> interactorImplementors,
        IReadOnlyDictionary<string, List<INamedTypeSymbol>> abstractValidatorByRequestModelName)
    {
        var interactorWithMissingSuffix = interactorImplementors
            .Where(x => !x.Interactor.Name.EndsWith("Interactor"))
            .Select(x =>
                new BadInteractorDiagnosticContext(x, GetValidatorsOrDefault(x)));

        return interactorWithMissingSuffix
            .Where(x => x.Interactor.Locations.Any())
            .Select(badInteractor => Diagnostic.Create(
                Rules.MissingInteractorSuffix,
                badInteractor.Interactor.Locations.First(),
                badInteractor.Interactor.Name)
            );

        List<INamedTypeSymbol> GetValidatorsOrDefault(InteractorImplementorContext x)
        {
            return abstractValidatorByRequestModelName.TryGetValue(x.RequestModel.Name, out var validators)
                ? validators
                : [];
        }
    }
}