using System.Linq.Expressions;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters.Properties;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

public interface IPropertyFilterComparisonVisitor<TEntity> where TEntity : IEntity
{
    Expression<Func<TEntity, bool>> Visit(OpenedFilter openedFilter);
    Expression<Func<TEntity, bool>> Visit(BalanceFilter balanceFilter);
    Expression<Func<TEntity, bool>> Visit(ClosedFilter closedFilter);
    Expression<Func<TEntity, bool>> Visit(SizeFilter sizeFilter);
    Expression<Func<TEntity, bool>> Visit(ResultFilter resultFilter);
}