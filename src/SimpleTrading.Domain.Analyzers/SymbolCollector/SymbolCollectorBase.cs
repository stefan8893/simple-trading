using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Analyzers.SymbolCollector;

public abstract class SymbolCollectorBase(
    CancellationToken cancellationToken)
    : SymbolVisitor
{
    private readonly HashSet<INamedTypeSymbol> _collector = new(SymbolEqualityComparer.Default);

    protected abstract bool SatisfiesFilterPredicate(INamedTypeSymbol symbol);

    public ImmutableArray<INamedTypeSymbol> CollectIn(INamespaceSymbol? symbol)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _collector.Clear();
        Visit(symbol);

        return _collector.ToImmutableArray();
    }

    public override void VisitAssembly(IAssemblySymbol assembly)
    {
        cancellationToken.ThrowIfCancellationRequested();

        assembly.GlobalNamespace.Accept(this);
    }

    public override void VisitNamespace(INamespaceSymbol @namespace)
    {
        foreach (var namespaceOrType in @namespace.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();

            namespaceOrType.Accept(this);
        }
    }

    public override void VisitNamedType(INamedTypeSymbol type)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (SatisfiesFilterPredicate(type))
            _collector.Add(type);

        var nestedTypes = type.GetTypeMembers();
        if (nestedTypes.IsDefaultOrEmpty)
            return;

        foreach (var nestedType in nestedTypes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            nestedType.Accept(this);
        }
    }
}