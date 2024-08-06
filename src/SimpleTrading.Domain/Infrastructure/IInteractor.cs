namespace SimpleTrading.Domain.Infrastructure;

public interface IInteractor<in TRequestModel, TResponseModel>
{
    Task<TResponseModel> Execute(TRequestModel model);
}