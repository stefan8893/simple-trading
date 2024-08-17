using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.Currencies.GetCurrencies;

public interface IGetCurrencies : IInteractor<GetCurrenciesRequestModel,
    OneOf<IReadOnlyList<GetCurrenciesResponseModel>, BadInput>>
{
}