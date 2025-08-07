using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Generators;

internal static class Rules
{
    private const string ConventionCategory = "Convention";

    public static readonly DiagnosticDescriptor MissingInteractorSuffix = new("ST0001",
        "Interactors must have the suffix 'Interactor'",
        "{0} must end with 'Interactor', since it implements 'IInteractor'",
        ConventionCategory,
        DiagnosticSeverity.Error,
        true,
        "By convention all interactors that implement IInteractor must have the suffix 'Interactor'.");
}