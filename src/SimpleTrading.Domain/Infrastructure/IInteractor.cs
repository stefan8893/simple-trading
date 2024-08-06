namespace SimpleTrading.Domain.Infrastructure;

public interface IInteractor<in TRequestModel, TResponse>
{
    Task<TResponse> Execute(TRequestModel model);
}