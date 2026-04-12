namespace SimpleTrading.Domain.Generators;

public static class InfrastructureSource
{
    public const string InteractorInterface =
        // lang=C#
        """
        using System;
        using System.Threading;
        using System.Threading.Tasks;

        namespace SimpleTrading.Domain.Infrastructure;

        public interface IInteractor<TResponseModel>
        {
            Task<TResponseModel> Execute(CancellationToken cancellationToken = default);
        }

        public interface IInteractor<in TRequestModel, TResponseModel>
        {
            Task<TResponseModel> Execute(TRequestModel requestModel, CancellationToken cancellationToken = default);
        }
        """;
}