namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

public static class Operator
{
    public const string EqualsTo = "eq";
    public const string NotEqualsTo = "ne";
    public const string GreaterThan = "gt";
    public const string GreaterThanOrEqualTo = "ge";
    public const string LessThan = "lt";
    public const string LessThanOrEqualTo = "le";

    public static readonly ISet<string> All =
        typeof(Operator).GetFields()
            .Where(x => x is {IsLiteral: true, IsInitOnly: false})
            .Select(x => (string) x.GetValue(null)!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
}