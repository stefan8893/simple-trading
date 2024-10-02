namespace SimpleTrading.Domain.Abstractions;

public interface IInteractor<TResponseModel>
{
    ValueTask<TResponseModel> Execute();
}

public interface IInteractor<in TRequestModel, TResponseModel>
{
    ValueTask<TResponseModel> Execute(TRequestModel requestModel);
}