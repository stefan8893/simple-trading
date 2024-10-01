using FluentValidation;
using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.Assets.GetAssets;

public class GetAssetsInteractor(IValidator<GetAssetsRequestModel> validator, IAssetRepository assetRepository)
    : InteractorBase, IInteractor<GetAssetsRequestModel, OneOf<IReadOnlyList<GetAssetsResponseModel>, BadInput>>
{
    public async Task<OneOf<IReadOnlyList<GetAssetsResponseModel>, BadInput>> Execute(GetAssetsRequestModel model)
    {
        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
            return BadInput(validationResult);

        var useSearchTerm = !string.IsNullOrWhiteSpace(model.SearchTerm);

        var result = useSearchTerm
            ? await assetRepository.Find(model.SearchTerm!)
            : await assetRepository.GetAll();

        return result
            .Select(x => new GetAssetsResponseModel(x.Id, x.Symbol, x.Name))
            .ToList();
    }
}