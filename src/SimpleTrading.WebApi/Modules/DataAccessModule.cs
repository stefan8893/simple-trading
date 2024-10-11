using System.Reflection;
using Autofac;
using SimpleTrading.DataAccess;
using SimpleTrading.DataAccess.PropertyFilterPredicates;
using SimpleTrading.DataAccess.Repositories;
using SimpleTrading.DataAccess.Sorting;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure.DataAccess;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;
using Module = Autofac.Module;

namespace SimpleTrading.WebApi.Modules;

public class DataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var dataAccessAssembly = typeof(RepositoryBase<>).Assembly;

        AddRepositories(builder, dataAccessAssembly);
        AddUnitOfWork(builder);
        AddPropertyFilter(builder, dataAccessAssembly);
        AddPropertySorting(builder);
        AddDbMasterData(builder);
    }

    private static void AddRepositories(ContainerBuilder builder, Assembly currentAssembly)
    {
        builder.RegisterAssemblyTypes(currentAssembly)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }

    private static void AddUnitOfWork(ContainerBuilder builder)
    {
        builder.Register<UowCommit>(ctx =>
            {
                var dbContext = ctx.Resolve<TradingDbContext>();
                return () => dbContext.SaveChangesAsync();
            })
            .InstancePerLifetimeScope();
    }

    private static void AddPropertyFilter(ContainerBuilder builder, Assembly currentAssembly)
    {
        builder.RegisterAssemblyTypes(currentAssembly)
            .AsClosedTypesOf(typeof(IFilterPredicate<>))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(currentAssembly)
            .AsClosedTypesOf(typeof(IValueParser<>))
            .AsImplementedInterfaces()
            .SingleInstance();
    }

    private static void AddPropertySorting(ContainerBuilder builder)
    {
        builder.Register<IReadOnlyDictionary<string, Func<Order, ISort<Trade>>>>(_ =>
                new Dictionary<string, Func<Order, ISort<Trade>>>(StringComparer.OrdinalIgnoreCase)
                {
                    [nameof(Trade.Opened)] = order => new SortByOpened(order),
                    [nameof(Trade.Closed)] = order => new SortByClosed(order),
                    [nameof(Trade.Balance)] = order => new SortByBalance(order),
                    [nameof(Trade.Size)] = order => new SortBySize(order),
                    [nameof(Trade.Result)] = order => new SortByResult(order)
                })
            .SingleInstance();
    }

    private static void AddDbMasterData(ContainerBuilder builder)
    {
        builder.RegisterType<DbMasterData>()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}