using System.Reflection;
using Autofac;
using SimpleTrading.DataAccess.PropertyFilterPredicates;
using SimpleTrading.DataAccess.Sorting;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure.DataAccess;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;
using Module = Autofac.Module;

namespace SimpleTrading.DataAccess;

public class DataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();

        builder.RegisterAssemblyTypes(currentAssembly)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.Register<UowCommit>(ctx =>
            {
                var dbContext = ctx.Resolve<TradingDbContext>();
                return () => dbContext.SaveChangesAsync();
            })
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(currentAssembly)
            .AsClosedTypesOf(typeof(IFilterPredicate<>))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(currentAssembly)
            .AsClosedTypesOf(typeof(IValueParser<>))
            .AsImplementedInterfaces()
            .SingleInstance();

        builder.Register<IReadOnlyDictionary<string, Func<Order, ISort<Trade>>>>(_ =>
                new Dictionary<string, Func<Order, ISort<Trade>>>(StringComparer.OrdinalIgnoreCase)
                {
                    [PropertyFilter.Opened] = order => new SortByOpened(order),
                    [PropertyFilter.Closed] = order => new SortByClosed(order),
                    [PropertyFilter.Balance] = order => new SortByBalance(order),
                    [PropertyFilter.Size] = order => new SortBySize(order),
                    [PropertyFilter.Result] = order => new SortByResult(order)
                })
            .SingleInstance();

        builder.RegisterType<DbMasterData>()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}