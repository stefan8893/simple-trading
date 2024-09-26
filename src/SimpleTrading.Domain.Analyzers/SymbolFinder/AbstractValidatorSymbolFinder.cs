using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Analyzers.SymbolFinder;

public class AbstractValidatorSymbolFinder(CancellationToken cancellationToken) : SymbolFinderBase(cancellationToken)
{
    protected override bool SatisfiesFilterPredicate(INamedTypeSymbol symbol)
    {
        return symbol.BaseType is {IsGenericType: true, MetadataName: "AbstractValidator`1", Arity: 1};
    }
}