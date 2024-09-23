namespace SimpleTrading.Domain.Abstractions;

public interface IInteractor<TResponseModel>
{
    Task<TResponseModel> Execute();
}

public interface IInteractor<in TRequestModel, TResponseModel>
{
    Task<TResponseModel> Execute(TRequestModel model);
}