namespace SimpleTrading.Domain.Generators;

public static class InfrastructureSource
{
    public const string InteractorInterface =
        // lang=C#
        $$"""
        namespace SimpleTrading.Domain.Infrastructure;

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