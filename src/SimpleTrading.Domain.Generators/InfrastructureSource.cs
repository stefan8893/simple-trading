namespace SimpleTrading.Domain.Generators;

public static class InfrastructureSource
{
    public const string Namespace = "SimpleTrading.Domain.Infrastructure";
    
    public const string ValidationResultMarkerAttribute =
        // lang=C#
        $$"""
        namespace {{Namespace}};

        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        public sealed class InteractorProxyValidationResultAttribute : Attribute
        {
            public InteractorProxyValidationResultAttribute() {}
        }
        """;

    public const string InteractorInterface =
        // lang=C#
        $$"""
        namespace {{Namespace}};

        public interface IInteractor<TResponseModel>
        {
            Task<TResponseModel> Execute();
        }

        public interface IInteractor<in TRequestModel, TResponseModel>
        {
            Task<TResponseModel> Execute(TRequestModel requestModel);
        }
        """;
}