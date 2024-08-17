using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.Assets.GetAssets;

public interface IGetAssets : IInteractor<GetAssetsRequestModel, OneOf<IReadOnlyList<GetAssetsResponseModel>, BadInput>>
{
}