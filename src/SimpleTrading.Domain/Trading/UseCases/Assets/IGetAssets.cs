using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.Assets;

public interface IGetAssets : IInteractor<GetAssetsRequestModel, OneOf<IReadOnlyList<GetAssetsResponseModel>, BadInput>>
{
}