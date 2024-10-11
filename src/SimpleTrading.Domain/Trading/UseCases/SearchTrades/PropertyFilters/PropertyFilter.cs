namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

public static class PropertyFilter
{
    public const string Opened = nameof(Trade.Opened);
    public const string Closed = nameof(Trade.Closed);
    public const string Balance = nameof(Trade.Balance);
    public const string Size = nameof(Trade.Size);
    public const string Result = nameof(Trade.Result);

    public static class Operator
    {
        public const string EqualTo = "eq";
        public const string NotEqualTo = "ne";
        public const string GreaterThan = "gt";
        public const string GreaterThanOrEqualTo = "ge";
        public const string LessThan = "lt";
        public const string LessThanOrEqualTo = "le";
    }
}