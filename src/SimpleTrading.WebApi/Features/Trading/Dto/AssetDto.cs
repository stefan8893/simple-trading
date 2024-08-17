using SimpleTrading.Domain.Trading.UseCases.Assets.GetAssets;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public class AssetDto
{
    public required Guid Id { get; init; }
    public required string Symbol { get; init; }
    public required string Name { get; init; }

    public static AssetDto From(GetAssetsResponseModel model)
    {
        return new AssetDto
        {
            Id = model.Id,
            Symbol = model.Symbol,
            Name = model.Name
        };
    }
}