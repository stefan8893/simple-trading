namespace SimpleTrading.Domain.Trading.UseCases.Currencies.GetCurrencies;

public record GetCurrenciesResponseModel(Guid Id, string IsoCode, string Name);