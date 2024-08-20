using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.DataAccess.PropertyFilterVisitors;
using SimpleTrading.DataAccess.Repositories;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        return services
            .AddScoped<ITradeRepository, TradeRepository>()
            .AddScoped<IAssetRepository, AssetRepository>()
            .AddScoped<IProfileRepository, ProfileRepository>()
            .AddScoped<ICurrencyRepository, CurrencyRepository>()
            .AddScoped<IUserSettingsRepository, UserSettingsRepository>()
            .AddScoped<UowCommit>(sp => () => sp.GetRequiredService<TradingDbContext>().SaveChangesAsync())
            .AddSingleton<IReadOnlyDictionary<string, IPropertyFilterComparisonVisitor<Trade>>>(sp =>
                new Dictionary<string, IPropertyFilterComparisonVisitor<Trade>>
                {
                    [Operator.EqualsTo] = new EqualToPropertyFilterVisitor(),
                    [Operator.NotEqualsTo] = new NotEqualToPropertyFilterVisitor(),
                    [Operator.GreaterThan] = new GreaterThanPropertyFilterVisitor(),
                    [Operator.GreaterThanOrEqualTo] = new GreaterThanOrEqualToPropertyFilterVisitor(),
                    [Operator.LessThan] = new LessThanPropertyFilterVisitor(),
                    [Operator.LessThanOrEqualTo] = new LessThanOrEqualToPropertyFilterVisitor()
                });
    }
}