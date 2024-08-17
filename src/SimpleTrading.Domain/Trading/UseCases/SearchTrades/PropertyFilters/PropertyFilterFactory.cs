using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

public static class PropertyFilterFactory
{
    private static readonly Dictionary<string, Func<string, string, IPropertyFilter<Trade>>> PropertyFilterByName =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(SupportedPropertyFilters.Opened)] = OpenedFilter.Create,
            [nameof(SupportedPropertyFilters.Closed)] = ClosedFilter.Create,
            [nameof(SupportedPropertyFilters.Balance)] = BalanceFilter.Create,
            [nameof(SupportedPropertyFilters.Size)] = SizeFilter.Create,
            [nameof(SupportedPropertyFilters.Result)] = ResultFilter.Create
        };

    public static IPropertyFilter<Trade> Create(string name, string @operator, string value)
    {
        return PropertyFilterByName[name](@operator, value);
    }

    public static bool CanCreateFilter(string propertyFilterName, string comparisonValue)
    {
        var canParse = new Dictionary<string, Func<string, bool>>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(SupportedPropertyFilters.Opened)] = OpenedFilter.CanParseComparisonValue,
            [nameof(SupportedPropertyFilters.Closed)] = ClosedFilter.CanParseComparisonValue,
            [nameof(SupportedPropertyFilters.Balance)] = BalanceFilter.CanParseComparisonValue,
            [nameof(SupportedPropertyFilters.Size)] = SizeFilter.CanParseComparisonValue,
            [nameof(SupportedPropertyFilters.Result)] = ResultFilter.CanParseComparisonValue
        };

        return canParse[propertyFilterName](comparisonValue);
    }
}