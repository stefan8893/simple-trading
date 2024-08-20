using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

public static class PropertyFilterFactory
{
    private static readonly Dictionary<string, Func<string, string, bool, IPropertyFilter<Trade>>>
        PropertyFilterByName =
            new(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(PropertyFilter.Opened)] = OpenedFilter.Create,
                [nameof(PropertyFilter.Closed)] = ClosedFilter.Create,
                [nameof(PropertyFilter.Balance)] = BalanceFilter.Create,
                [nameof(PropertyFilter.Size)] = SizeFilter.Create,
                [nameof(PropertyFilter.Result)] = ResultFilter.Create
            };

    public static IPropertyFilter<Trade> Create(string name, string @operator, string value, bool isLiteral)
    {
        return PropertyFilterByName[name](@operator, value, isLiteral);
    }

    public static bool CanCreate(string propertyFilterName, string comparisonValue, bool isLiteral)
    {
        var canParse = new Dictionary<string, Func<bool>>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(PropertyFilter.Opened)] = () => OpenedFilter.TryParseComparisonValue(comparisonValue, out _),
            [nameof(PropertyFilter.Closed)] =
                () => ClosedFilter.TryParseComparisonValue(comparisonValue, isLiteral, out _),
            [nameof(PropertyFilter.Balance)] =
                () => BalanceFilter.TryParseComparisonValue(comparisonValue, isLiteral, out _),
            [nameof(PropertyFilter.Size)] = () => SizeFilter.TryParseComparisonValue(comparisonValue, out _),
            [nameof(PropertyFilter.Result)] =
                () => ResultFilter.TryParseComparisonValue(comparisonValue, isLiteral, out _)
        };

        return canParse[propertyFilterName]();
    }

    public static bool IsNullComparisonForbidden(string property, string @operator)
    {
        // only consider nullable properties
        var nullComparisonForbidden =
            new Dictionary<(string, string), bool>(new ValueTupleStringComparerOrdinalIgnoreCase())
            {
                [(PropertyFilter.Closed, Operator.GreaterThan)] = true,
                [(PropertyFilter.Closed, Operator.GreaterThanOrEqualTo)] = true,
                [(PropertyFilter.Closed, Operator.LessThan)] = true,
                [(PropertyFilter.Closed, Operator.LessThanOrEqualTo)] = true,

                [(PropertyFilter.Balance, Operator.GreaterThan)] = true,
                [(PropertyFilter.Balance, Operator.GreaterThanOrEqualTo)] = true,
                [(PropertyFilter.Balance, Operator.LessThan)] = true,
                [(PropertyFilter.Balance, Operator.LessThanOrEqualTo)] = true,

                [(PropertyFilter.Result, Operator.GreaterThan)] = true,
                [(PropertyFilter.Result, Operator.GreaterThanOrEqualTo)] = true,
                [(PropertyFilter.Result, Operator.LessThan)] = true,
                [(PropertyFilter.Result, Operator.LessThanOrEqualTo)] = true
            };

        return nullComparisonForbidden.TryGetValue((property, @operator), out var result) && result;
    }
}

file class ValueTupleStringComparerOrdinalIgnoreCase : IEqualityComparer<(string, string)>
{
    public bool Equals((string, string) x, (string, string) y)
    {
        return x.Item1.Equals(y.Item1, StringComparison.OrdinalIgnoreCase)
               && x.Item2.Equals(y.Item2, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode((string, string) obj)
    {
        var item1Hash = StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Item1);
        var item2Hash = StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Item2);

        return HashCode.Combine(item1Hash, item2Hash);
    }
}