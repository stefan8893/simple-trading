using SimpleTrading.Domain.Trading.UseCases.Currencies;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public class CurrencyDto
{
    public required Guid Id { get; init; }
    public required string IsoCode { get; init; }
    public required string Name { get; init; }

    public static CurrencyDto From(GetCurrenciesResponseModel model)
    {
        return new CurrencyDto
        {
            Id = model.Id,
            IsoCode = model.IsoCode,
            Name = model.Name
        };
    }
}