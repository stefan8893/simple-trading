using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.Assets;

public class GetAssetsInteractor(IValidator<GetAssetsRequestModel> validator, TradingDbContext dbContext)
    : BaseInteractor, IGetAssets
{
    public async Task<OneOf<IReadOnlyList<GetAssetsResponseModel>, BadInput>> Execute(GetAssetsRequestModel model)
    {
        var validation = await validator.ValidateAsync(model);
        if (!validation.IsValid)
            return BadInput(validation);

        var useSearchTerm = !string.IsNullOrWhiteSpace(model.SearchTerm);
        var searchTerm = model.SearchTerm?.Trim().ToLower();

        var result = await dbContext.Assets
            .Where(x => !useSearchTerm || EF.Functions.Like(x.Name.ToLower(), $"%{searchTerm}%"))
            .ToListAsync();

        return result
            .Select(x => new GetAssetsResponseModel(x.Id, x.Symbol, x.Name))
            .ToList();
    }
}