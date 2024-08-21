namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

public static class PropertyFilter
{
    public const string Opened = nameof(Opened);
    public const string Closed = nameof(Closed);
    public const string Balance = nameof(Balance);
    public const string Size = nameof(Size);
    public const string Result = nameof(Result);
    
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