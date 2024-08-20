using System.Linq.Expressions;
using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

public interface IPropertyFilter<TEntity> where TEntity : IEntity
{
    string PropertyName { get; }
    public string Operator { get; }

    Expression<Func<TEntity, bool>> GetPredicate(IPropertyFilterComparisonVisitor<TEntity> visitor);
}

public interface IPropertyFilter<TEntity, out TProperty> : IPropertyFilter<TEntity>
    where TEntity : IEntity
{
    TProperty ComparisonValue { get; }
}