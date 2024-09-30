﻿namespace SimpleTrading.Domain.Generators;

public static class SourceTemplates
{
    public static string CreateProxy(InteractorContext ctx)
    {
        var requestModelFullQualifiedType = ctx.RequestModel is null
            ? string.Empty
            : ctx.RequestModel.ToDisplayString();

        var responseModelFullQualifiedTyped = ctx.ResponseModel.ToDisplayString();

        List<string> namespaces =
        [
            "System",
            "System.Threading.Tasks",
            "System.Diagnostics",
            ctx.Interactor.ContainingNamespace.ToDisplayString()
        ];

        var usingStatements = namespaces
            .Distinct()
            .Select(static x => $"using {x};");

        var requestModelParameter = string.IsNullOrWhiteSpace(requestModelFullQualifiedType)
            ? string.Empty
            : $"{requestModelFullQualifiedType} requestModel";

        // lang=C#
        return $$"""
                       //----------------------
                       // <auto-generated>
                       //     Generated using Source Generators
                       // </auto-generated>
                       //----------------------

                       {{string.Join("\n\r", usingStatements)}}

                       namespace {{ctx.Interactor.ContainingNamespace.ToDisplayString()}}
                       {
                           public interface {{ctx.InteractorInterfaceName}}
                           {
                               Task<{{responseModelFullQualifiedTyped}}> Execute({{requestModelParameter}});
                           }
                           
                           public sealed class {{ctx.InteractorProxyName}} : {{ctx.InteractorInterfaceName}}
                           {
                               private readonly {{ctx.ClosedInteractorInterface.ToDisplayString()}}  _interactor;
                               
                               public {{ctx.InteractorProxyName}}(
                                        {{ctx.ClosedInteractorInterface.ToDisplayString()}} interactor)
                               {
                                   _interactor = interactor;
                               }
                               
                               [DebuggerStepThrough]
                               public Task<{{responseModelFullQualifiedTyped}}> Execute({{requestModelParameter}}) 
                               {
                                   return {{IfRequestModelExists("_interactor.Execute(requestModel);",
                                       "_interactor.Execute();")}}
                               }
                           }    
                       }
                       """;

        string IfRequestModelExists(string onRequestModelExists, string? otherwise = null)
        {
            return ctx.RequestModel is not null ? onRequestModelExists : otherwise ?? string.Empty;
        }
    }
}