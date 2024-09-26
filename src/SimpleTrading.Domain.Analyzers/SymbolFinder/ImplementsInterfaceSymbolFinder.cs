using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Analyzers.SymbolFinder;

public class ImplementsInterfaceSymbolFinder(CancellationToken cancellationToken, INamedTypeSymbol interfaceType)
    : SymbolFinderBase(cancellationToken)
{
    protected override bool SatisfiesFilterPredicate(INamedTypeSymbol symbol)
    {
        return symbol
            .AllInterfaces
            .Any(x => x.Name == interfaceType.Name &&
                      x.MetadataName == interfaceType.MetadataName &&
                      x.IsGenericType == interfaceType.IsGenericType &&
                      x.Arity == interfaceType.Arity);
    }
}