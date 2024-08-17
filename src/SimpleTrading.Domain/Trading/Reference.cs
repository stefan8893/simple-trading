using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.Trading;

public enum ReferenceType
{
    TradingView = 0,
    Other = 1
}

public class Reference  : IEntity
{
    public required Guid Id { get; set; }
    public required Guid TradeId { get; set; }
    public virtual required Trade Trade { get; set; }
    public required ReferenceType Type { get; set; }
    public required Uri Link { get; set; }
    public string? Notes { get; set; }
    public required DateTime Created { get; init; }
}
