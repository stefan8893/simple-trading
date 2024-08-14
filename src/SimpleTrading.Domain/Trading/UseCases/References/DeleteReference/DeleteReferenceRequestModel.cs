namespace SimpleTrading.Domain.Trading.UseCases.References.DeleteReference;

public record DeleteReferenceRequestModel(Guid TradeId, Guid ReferenceId);