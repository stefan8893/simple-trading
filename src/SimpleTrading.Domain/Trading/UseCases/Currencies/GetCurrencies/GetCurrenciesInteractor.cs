using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.Currencies.GetCurrencies;

using GetCurrenciesResponse = OneOf<IReadOnlyList<GetCurrenciesResponseModel>, BadInput>;

[UsedImplicitly]
public class GetCurrenciesInteractor(
    ICurrencyRepository currencyRepository)
    : InteractorBase, IInteractor<GetCurrenciesRequestModel, GetCurrenciesResponse>
{
    public async Task<GetCurrenciesResponse> Execute(
        GetCurrenciesRequestModel model)
    {
        var useSearchTerm = !string.IsNullOrWhiteSpace(model.SearchTerm);

        var result = useSearchTerm
            ? await currencyRepository.Find(model.SearchTerm!)
            : await currencyRepository.GetAll();

        return result
            .Select(x => new GetCurrenciesResponseModel(x.Id, x.IsoCode, x.Name))
            .ToList();
    }
}