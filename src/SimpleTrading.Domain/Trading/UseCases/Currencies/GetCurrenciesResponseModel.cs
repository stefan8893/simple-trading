namespace SimpleTrading.Domain.Trading.UseCases.Currencies;

public record GetCurrenciesResponseModel(Guid Id, string IsoCode, string Name);