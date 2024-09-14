using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.Trading;

public class Asset : IEntity
{
    public required Guid Id { get; init; }
    public required string Symbol { get; init; }
    public required string Name { get; init; }
    public required DateTime Created { get; init; }
}