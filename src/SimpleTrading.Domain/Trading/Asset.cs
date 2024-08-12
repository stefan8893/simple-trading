namespace SimpleTrading.Domain.Trading;

public class Asset
{
    public required Guid Id { get; init; }
    public required string Symbol { get; init; }
    public required string Name { get; init; }
    public required DateTime Created { get; init; }
}