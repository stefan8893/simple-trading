using FluentValidation;
using OneOf;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.Currencies.GetCurrencies;

public class GetCurrenciesInteractor(
    IValidator<GetCurrenciesRequestModel> validator,
    ICurrencyRepository currencyRepository)
    : BaseInteractor, IGetCurrencies
{
    public async Task<OneOf<IReadOnlyList<GetCurrenciesResponseModel>, BadInput>> Execute(
        GetCurrenciesRequestModel model)
    {
        var validation = await validator.ValidateAsync(model);
        if (!validation.IsValid)
            return BadInput(validation);

        var useSearchTerm = !string.IsNullOrWhiteSpace(model.SearchTerm);

        var result = useSearchTerm
            ? await currencyRepository.Find(model.SearchTerm!)
            : await currencyRepository.GetAll();

        return result
            .Select(x => new GetCurrenciesResponseModel(x.Id, x.IsoCode, x.Name))
            .ToList();
    }
}