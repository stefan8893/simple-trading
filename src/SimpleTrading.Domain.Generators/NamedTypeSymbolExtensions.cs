using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Generators;

public static class NamedTypeSymbolExtensions
{
    extension(INamedTypeSymbol namedTypeSymbol)
    {
        public string GetDisplayName()
        {
            var displayFormat = new SymbolDisplayFormat(
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
            );

            return namedTypeSymbol.ToDisplayString(displayFormat);
        }

        public IEnumerable<string> GetAllNamespaces()
        {
            return CollectNamespaces(namedTypeSymbol)
                .Distinct();

            IEnumerable<string> CollectNamespaces(ITypeSymbol? symbol)
            {
                if (symbol is null)
                    yield break;

                var currentNamespace = symbol.ContainingNamespace?.ToDisplayString();

                if (!string.IsNullOrEmpty(currentNamespace))
                    yield return currentNamespace!;

                if (symbol is INamedTypeSymbol named)
                    foreach (var ns in named.TypeArguments.SelectMany(CollectNamespaces))
                        yield return ns;
            }
        }
    }
}