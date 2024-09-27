// Do not change namespace, or keep in sync with 'IInteractor' in SimpleTrading.Domain.Abstractions namespace 
// ReSharper disable once CheckNamespace

namespace SimpleTrading.Domain.Abstractions;

public interface IInteractor<TResponseModel>
{
    Task<TResponseModel> Execute();
}

public interface IInteractor<in TRequestModel, TResponseModel>
{
    Task<TResponseModel> Execute(TRequestModel model);
}