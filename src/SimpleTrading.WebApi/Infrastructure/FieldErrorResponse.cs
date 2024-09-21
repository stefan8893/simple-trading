namespace SimpleTrading.WebApi.Infrastructure;

public class FieldError
{
    public required string Identifier { get; init; }
    public required IReadOnlyList<string> Messages { get; init; }
}

public class FieldErrorResponse
{
    public required IReadOnlyList<FieldError> Errors { get; init; }
}