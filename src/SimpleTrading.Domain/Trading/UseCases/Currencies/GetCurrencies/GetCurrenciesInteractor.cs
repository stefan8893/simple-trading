using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.Currencies.GetCurrencies;

[UsedImplicitly]
public class GetCurrenciesInteractor(
    ICurrencyRepository currencyRepository)
    : InteractorBase, IInteractor<GetCurrenciesRequestModel, IReadOnlyList<GetCurrenciesResponseModel>>
{
    public async Task<IReadOnlyList<GetCurrenciesResponseModel>> Execute(
        GetCurrenciesRequestModel model, CancellationToken cancellationToken)
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