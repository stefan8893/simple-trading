using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SimpleTrading.Domain.Generators;

public class InteractorContext(INamedTypeSymbol interactor, INamedTypeSymbol closedInteractorInterface, INamedTypeSymbol responseModel)
{
    public INamedTypeSymbol Interactor { get; } = interactor;
    public INamedTypeSymbol ClosedInteractorInterface { get; } = closedInteractorInterface;
    public INamedTypeSymbol? RequestModel { get; set; }
    public INamedTypeSymbol ResponseModel { get; } = responseModel;

    public string InteractorName => Interactor.Name.Replace("Interactor", "");
    public string InteractorInterfaceName => $"I{Interactor.Name.Replace("Interactor", "")}";
    public string InteractorProxyName => $"{Interactor.Name}Proxy";
}