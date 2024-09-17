namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ValueParser.Infrastructure;

// Unfortunately, the DI Container cannot distinguish between Type? and Type, e.g. string? and string.
// Because nullable reference types are a compile time feature and the DI Container resolves dependencies at runtime.
// This wrapper is used to mimic nullable reference types at runtime.
// Take a look at the NullableResultValueParser to see its usage.
public record NullableReference<T> where T : class?
{
    public static readonly NullableReference<T> Null = new();
    public T? Value { get; init; }
    public bool IsNull => Value is null;

    public static NullableReference<T> From(T value)
    {
        return new NullableReference<T> {Value = value};
    }
}