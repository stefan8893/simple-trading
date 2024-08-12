namespace SimpleTrading.WebApi.Infrastructure;

public class ErrorResponse
{
    public required IReadOnlyList<FieldError> FieldErrors { get; init; } = [];
    public required IReadOnlyList<string> CommonErrors { get; init; } = [];
}