﻿using FluentValidation;
using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.Currencies.GetCurrencies;

using GetCurrenciesResponse = OneOf<IReadOnlyList<GetCurrenciesResponseModel>, BadInput>;

[UsedImplicitly]
public class GetCurrenciesInteractor(
    IValidator<GetCurrenciesRequestModel> validator,
    ICurrencyRepository currencyRepository)
    : InteractorBase, IInteractor<GetCurrenciesRequestModel, GetCurrenciesResponse>
{
    public async Task<GetCurrenciesResponse> Execute(
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