using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

public static class PropertyFilterFactory
{
    public static IPropertyFilter<Trade> Create(string name, string @operator, string value, bool isLiteral)
    {
        var propertyFilterByName =
            new Dictionary<string, Func<IPropertyFilter<Trade>>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(PropertyFilter.Opened)] = () => OpenedFilter.Create(@operator, value),
                [nameof(PropertyFilter.Closed)] = () => ClosedFilter.Create(@operator, value, isLiteral),
                [nameof(PropertyFilter.Balance)] = () => BalanceFilter.Create(@operator, value, isLiteral),
                [nameof(PropertyFilter.Size)] = () => SizeFilter.Create(@operator, value),
                [nameof(PropertyFilter.Result)] = () => ResultFilter.Create(@operator, value, isLiteral)
            };

        return propertyFilterByName[name]();
    }

    public static bool CanCreate(string propertyFilter, string comparisonValue, bool isLiteral)
    {
        var canParse = new Dictionary<string, Func<bool>>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(PropertyFilter.Opened)] = () => OpenedFilter.TryParseValue(comparisonValue, out _),
            [nameof(PropertyFilter.Closed)] = () => ClosedFilter.TryParseValue(comparisonValue, isLiteral, out _),
            [nameof(PropertyFilter.Balance)] = () => BalanceFilter.TryParseValue(comparisonValue, isLiteral, out _),
            [nameof(PropertyFilter.Size)] = () => SizeFilter.TryParseValue(comparisonValue, out _),
            [nameof(PropertyFilter.Result)] = () => ResultFilter.TryParseValue(comparisonValue, isLiteral, out _)
        };

        return canParse[propertyFilter]();
    }

    public static bool IsNullComparisonForbidden(string property, string @operator)
    {
        // only consider nullable properties
        var nullComparisonForbidden =
            new HashSet<(string, string)>(new ValueTupleStringComparerOrdinalIgnoreCase())
            {
                (PropertyFilter.Closed, Operator.GreaterThan),
                (PropertyFilter.Closed, Operator.GreaterThanOrEqualTo),
                (PropertyFilter.Closed, Operator.LessThan),
                (PropertyFilter.Closed, Operator.LessThanOrEqualTo),

                (PropertyFilter.Balance, Operator.GreaterThan),
                (PropertyFilter.Balance, Operator.GreaterThanOrEqualTo),
                (PropertyFilter.Balance, Operator.LessThan),
                (PropertyFilter.Balance, Operator.LessThanOrEqualTo),

                (PropertyFilter.Result, Operator.GreaterThan),
                (PropertyFilter.Result, Operator.GreaterThanOrEqualTo),
                (PropertyFilter.Result, Operator.LessThan),
                (PropertyFilter.Result, Operator.LessThanOrEqualTo)
            };

        return nullComparisonForbidden.Contains((property, @operator));
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