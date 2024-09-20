namespace SimpleTrading.WebApi.Infrastructure;

public class ErrorResponse
{
    public required IReadOnlyList<string> Reasons { get; init; }
}