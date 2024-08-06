namespace SimpleTrading.Domain.Trading;

public enum ReferenceType
{
    TradingViewLink = 0,
    Other = 1
}

public class Reference
{
    public required Guid Id { get; set; }
    public required Guid TradeId { get; set; }
    public required Trade Trade { get; set; }
    public required ReferenceType Type { get; init; }
    public required Uri Link { get; set; }
    public string? Notes { get; set; }
    public required DateTime CreatedAt { get; init; }
}
