namespace SimpleTrading.Domain.Trading.UseCases.Assets.GetAssets;

public record GetAssetsResponseModel(Guid Id, string Symbol, string Name);