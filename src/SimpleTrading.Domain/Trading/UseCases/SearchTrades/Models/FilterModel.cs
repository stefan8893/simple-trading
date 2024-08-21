namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;

public record FilterModel
{
    public required string PropertyName { get; init; }
    public required string Operator { get; init; }
    public required string ComparisonValue { get; init; }
    public required bool IsLiteral { get; init; }
}