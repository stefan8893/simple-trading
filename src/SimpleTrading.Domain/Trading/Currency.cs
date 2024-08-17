using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.Trading;

public class Currency : IEntity
{
    public required string IsoCode { get; init; }
    public required string Name { get; set; }
    public virtual required Guid Id { get; init; }
    public required DateTime Created { get; init; }
}