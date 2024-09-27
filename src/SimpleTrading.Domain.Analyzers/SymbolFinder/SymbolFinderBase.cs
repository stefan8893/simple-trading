using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Analyzers.SymbolFinder;

public abstract class SymbolFinderBase(
    CancellationToken cancellationToken)
    : SymbolVisitor
{
    private readonly HashSet<INamedTypeSymbol> _collector = new(SymbolEqualityComparer.Default);

    public IImmutableList<INamedTypeSymbol> Result => _collector.ToImmutableList();
    protected abstract bool SatisfiesFilterPredicate(INamedTypeSymbol symbol);

    public void FindIn(ISymbol? symbol)
    {
        Visit(symbol);
    }

    public override void VisitNamespace(INamespaceSymbol symbol)
    {
        foreach (var namespaceOrType in symbol.GetMembers())
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            namespaceOrType.Accept(this);
        }
    }

    public override void VisitNamedType(INamedTypeSymbol type)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (SatisfiesFilterPredicate(type))
        {
            _collector.Add(type);
            return;
        }

        var nestedTypes = type.GetTypeMembers();

        if (nestedTypes.IsDefaultOrEmpty)
            return;

        foreach (var nestedType in nestedTypes)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            nestedType.Accept(this);
        }
    }
}