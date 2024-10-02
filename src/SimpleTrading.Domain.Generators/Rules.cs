using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Generators;

public static class Rules
{
    private const string ConventionCategory = "Convention";

    public static readonly DiagnosticDescriptor MissingInteractorSuffix = new("ST0003",
        "Interactors must have the suffix 'Interactor'",
        "{0} must end with 'Interactor', since it implements 'IInteractor'",
        ConventionCategory,
        DiagnosticSeverity.Error,
        true,
        "In order to add all Interactor Proxies to the DI container, they must end with 'Interactor', because DI registrations are done by conventions.");
}