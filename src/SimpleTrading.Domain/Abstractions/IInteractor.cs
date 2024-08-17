namespace SimpleTrading.Domain.Abstractions;

public interface IInteractor<in TRequestModel, TResponse>
{
    Task<TResponse> Execute(TRequestModel model);
}