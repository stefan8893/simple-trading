using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Analyzers;

internal static class Rules
{
    private const string UsageCategory = "Usage";

    public static readonly DiagnosticDescriptor MissingBadInputCase = new("ST0001",
        "Missing 'BadInput' case in response model",
        "Response model type 'OneOf' does not contain a case for 'BadInput', but this is required since there is a validator for '{0}'",
        UsageCategory,
        DiagnosticSeverity.Error,
        true,
        "The validation is done automatically if there exists a validator for the request model. If that validation fails the interactor is not being called and the caller receives the response model directly, that is the 'OneOf<BadInput>'. You must provide the 'BadInput' case in the signature of your use case interactor.");

    public static readonly DiagnosticDescriptor ResponseModelTypeIsNotOneOf = new("ST0002",
        "Response model is not of type 'OneOf' including 'BadInput' case",
        "Response model must be of type 'OneOf' including a 'BadInput' case, because there is a validator for '{0}'",
        UsageCategory,
        DiagnosticSeverity.Error,
        true,
        "The validation is done automatically if there exists a validator for the request model. If that validation fails the interactor is not being called and the caller receives the response model directly, that is the 'OneOf<BadInput>'. You must provide the 'BadInput' case in the signature of your use case interactor.");
}