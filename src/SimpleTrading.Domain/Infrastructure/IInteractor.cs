namespace SimpleTrading.Domain.Infrastructure;

public interface IInteractor<TResponseModel>
{
    Task<TResponseModel> Execute();
}

public interface IInteractor<in TRequestModel, TResponseModel>
{
    Task<TResponseModel> Execute(TRequestModel requestModel);
}