namespace SimpleTrading.WebApi.Infrastructure;

public class FieldErrorResponse
{
    public required IReadOnlyList<FieldError> Errors { get; init; }
}