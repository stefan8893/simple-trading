namespace SimpleTrading.WebApi.Infrastructure;

public class FieldError
{
    public required string Identifier { get; init; }
    public required IReadOnlyList<string> Reasons { get; init; }
}