using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.Assets.GetAssets;

[UsedImplicitly]
public class GetAssetsInteractor(IAssetRepository assetRepository)
    : InteractorBase, IInteractor<GetAssetsRequestModel, OneOf<IReadOnlyList<GetAssetsResponseModel>, BadInput>>
{
    public async Task<OneOf<IReadOnlyList<GetAssetsResponseModel>, BadInput>> Execute(GetAssetsRequestModel model)
    {
        var useSearchTerm = !string.IsNullOrWhiteSpace(model.SearchTerm);

        var result = useSearchTerm
            ? await assetRepository.Find(model.SearchTerm!)
            : await assetRepository.GetAll();

        return result
            .Select(x => new GetAssetsResponseModel(x.Id, x.Symbol, x.Name))
            .ToList();
    }
}