using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Analyzers.SymbolCollector;

public class ImplementsInterfaceSymbolCollector(CancellationToken cancellationToken, INamedTypeSymbol interfaceType)
    : SymbolCollectorBase(cancellationToken)
{
    protected override bool SatisfiesFilterPredicate(INamedTypeSymbol symbol)
    {
        return symbol
            .AllInterfaces
            .Any(x => x.Name == interfaceType.Name &&
                      x.MetadataName == interfaceType.MetadataName &&
                      x.IsGenericType == interfaceType.IsGenericType &&
                      x.Arity == interfaceType.Arity &&
                      x.ContainingNamespace.Equals(interfaceType.ContainingNamespace, SymbolEqualityComparer.Default));
    }
}