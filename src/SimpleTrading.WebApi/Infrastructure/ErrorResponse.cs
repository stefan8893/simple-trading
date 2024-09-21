namespace SimpleTrading.WebApi.Infrastructure;

public class ErrorResponse
{
    public required IReadOnlyList<string> Messages { get; init; }
}