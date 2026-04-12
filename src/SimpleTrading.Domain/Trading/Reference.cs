using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading;


public class Reference  : IEntity
{
    public required Guid Id { get; set; }
    public required Guid TradeId { get; set; }
    public virtual required Trade Trade { get; set; }
    public required Uri Link { get; set; }
    public string? Notes { get; set; }
    public required DateTime Created { get; init; }
}
