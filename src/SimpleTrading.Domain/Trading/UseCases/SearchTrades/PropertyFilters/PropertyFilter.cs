namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

public static class PropertyFilter
{
    public const string Opened = nameof(Opened);
    public const string Closed = nameof(Closed);
    public const string Balance = nameof(Balance);
    public const string Size = nameof(Size);
    public const string Result = nameof(Result);

    public static readonly ISet<string> All =
        typeof(PropertyFilter).GetFields()
            .Where(x => x is {IsLiteral: true, IsInitOnly: false})
            .Select(x => (string) x.GetValue(null)!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
}