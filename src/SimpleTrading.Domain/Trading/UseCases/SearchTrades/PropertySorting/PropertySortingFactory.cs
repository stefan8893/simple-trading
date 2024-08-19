using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Abstractions.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertySorting;

public static class PropertySortingFactory
{
    public static readonly IReadOnlyDictionary<string, Func<Order, ISort<Trade>>> SortingByProperty =
        new Dictionary<string, Func<Order, ISort<Trade>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Opened"] = o => new SortByOpened(o),
            ["Closed"] = o => new SortByClosed(o),
            ["Balance"] = o => new SortByBalance(o),
            ["Size"] = o => new SortBySize(o),
            ["Result"] = o => new SortByResult(o)
        };

    public static readonly IReadOnlySet<string> SupportedSortingProperties = SortingByProperty
        .Select(x => x.Key)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);
}