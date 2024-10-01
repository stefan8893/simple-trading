using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Analyzers;

public class BadInteractorDiagnosticContext(
    InteractorImplementorContext interactorImplementorContext,
    IReadOnlyList<INamedTypeSymbol> validators)
    : InteractorImplementorContext(
        interactorImplementorContext.Interactor,
        interactorImplementorContext.RequestModel,
        interactorImplementorContext.ResponseModel)
{
    public IReadOnlyList<INamedTypeSymbol> Validators { get; } = validators;
}