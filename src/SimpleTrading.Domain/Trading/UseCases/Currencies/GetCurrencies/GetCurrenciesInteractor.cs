using FluentValidation;
using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.Currencies.GetCurrencies;

public class GetCurrenciesInteractor(
    IValidator<GetCurrenciesRequestModel> validator,
    ICurrencyRepository currencyRepository)
    : InteractorBase, IInteractor<GetCurrenciesRequestModel,
        OneOf<IReadOnlyList<GetCurrenciesResponseModel>, BadInput>>
{
    public async Task<OneOf<IReadOnlyList<GetCurrenciesResponseModel>, BadInput>> Execute(
        GetCurrenciesRequestModel model)
    {
        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
            return BadInput(validationResult);

        var useSearchTerm = !string.IsNullOrWhiteSpace(model.SearchTerm);

        var result = useSearchTerm
            ? await currencyRepository.Find(model.SearchTerm!)
            : await currencyRepository.GetAll();

        return result
            .Select(x => new GetCurrenciesResponseModel(x.Id, x.IsoCode, x.Name))
            .ToList();
    }
}