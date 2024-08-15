using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.Currencies;

public interface IGetCurrencies : IInteractor<GetCurrenciesRequestModel,
    OneOf<IReadOnlyList<GetCurrenciesResponseModel>, BadInput>>
{
}