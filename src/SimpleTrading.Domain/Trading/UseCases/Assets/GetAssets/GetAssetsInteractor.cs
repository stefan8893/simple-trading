using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.Assets.GetAssets;

[UsedImplicitly]
public class GetAssetsInteractor(IAssetRepository assetRepository)
    : InteractorBase, IInteractor<GetAssetsRequestModel, IReadOnlyList<GetAssetsResponseModel>>
{
    public async Task<IReadOnlyList<GetAssetsResponseModel>> Execute(GetAssetsRequestModel model, CancellationToken cancellationToken)
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