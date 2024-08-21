using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.DataAccess.PropertyFilterPredicates;
using SimpleTrading.DataAccess.Repositories;
using SimpleTrading.DataAccess.Sorting;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services
            .AddScoped<ITradeRepository, TradeRepository>()
            .AddScoped<IAssetRepository, AssetRepository>()
            .AddScoped<IProfileRepository, ProfileRepository>()
            .AddScoped<ICurrencyRepository, CurrencyRepository>()
            .AddScoped<IUserSettingsRepository, UserSettingsRepository>();

        services
            .AddScoped<UowCommit>(sp => () => sp.GetRequiredService<TradingDbContext>().SaveChangesAsync());

        services.Scan(scan =>
            scan.FromAssemblyOf<TradingDbContext>()
                .AddClasses(f => f.AssignableTo(typeof(IFilterPredicate<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.Scan(scan =>
            scan.FromAssemblyOf<TradingDbContext>()
                .AddClasses(f => f.AssignableTo(typeof(IValueParser<>)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        services.AddSingleton<IReadOnlyDictionary<string, Func<Order, ISort<Trade>>>>(sp => 
            new Dictionary<string, Func<Order, ISort<Trade>>>(StringComparer.OrdinalIgnoreCase)
        {
            [PropertyFilter.Opened] = order => new SortByOpened(order),
            [PropertyFilter.Closed] = order => new SortByClosed(order),
            [PropertyFilter.Balance] = order => new SortByBalance(order),
            [PropertyFilter.Size] = order => new SortBySize(order),
            [PropertyFilter.Result] = order => new SortByResult(order),
        });

        return services;
    }
}