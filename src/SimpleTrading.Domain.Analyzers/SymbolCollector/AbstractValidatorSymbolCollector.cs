using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Analyzers.SymbolCollector;

public class AbstractValidatorSymbolCollector(CancellationToken cancellationToken)
    : SymbolCollectorBase(cancellationToken)
{
    protected override bool SatisfiesFilterPredicate(INamedTypeSymbol symbol)
    {
        return symbol.BaseType is
        {
            IsGenericType: true, MetadataName: "AbstractValidator`1", Arity: 1,
            ContainingNamespace.MetadataName: "FluentValidation"
        };
    }
}