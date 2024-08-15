namespace SimpleTrading.Domain.Trading.UseCases.Assets;

public record GetAssetsResponseModel(Guid Id, string Symbol, string Name);