using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.Currencies;

public class GetCurrenciesInteractor(IValidator<GetCurrenciesRequestModel> validator, TradingDbContext dbContext)
    : BaseInteractor, IGetCurrencies
{
    public async Task<OneOf<IReadOnlyList<GetCurrenciesResponseModel>, BadInput>> Execute(
        GetCurrenciesRequestModel model)
    {
        var validation = await validator.ValidateAsync(model);
        if (!validation.IsValid)
            return BadInput(validation);

        var useSearchTerm = !string.IsNullOrWhiteSpace(model.SearchTerm);
        var searchTerm = model.SearchTerm?.Trim().ToLower();

        var result = await dbContext.Currencies
            .Where(x => !useSearchTerm || EF.Functions.Like(x.Name.ToLower(), $"%{searchTerm}%"))
            .ToListAsync();

        return result
            .Select(x => new GetCurrenciesResponseModel(x.Id, x.IsoCode, x.Name))
            .ToList();
    }
}