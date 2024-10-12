using System.Linq.Expressions;

namespace SimpleTrading.Domain.Infrastructure.Filter;

public interface IFilterPredicate<TEntity> where TEntity : IEntity
{
    bool Match(string property);
    bool Match(string property, string @operator);

    bool CanParse(string comparisonValue, bool isLiteral);

    Expression<Func<TEntity, bool>> GetPredicate(string comparisonValue, bool isLiteral);
}