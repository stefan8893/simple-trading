using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Analyzers;

public class BadInteractorDiagnosticContext(InteractorImplementor interactorImplementor, IReadOnlyList<INamedTypeSymbol> validators) 
    : InteractorImplementor(
    interactorImplementor.Interactor,
    interactorImplementor.RequestModel,
    interactorImplementor.ResponseModel)
{
    public IReadOnlyList<INamedTypeSymbol> Validators { get;} = validators;
}