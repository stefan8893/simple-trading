using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading;

public class Currency : IEntity
{
    public required Guid Id { get; init; }
    public required string IsoCode { get; init; }
    public required string Name { get; init; }
    public required DateTime Created { get; init; }
}