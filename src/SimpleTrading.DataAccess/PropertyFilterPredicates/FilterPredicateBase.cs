using System.Linq.Expressions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Filter;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates;

public abstract class FilterPredicateBase<TEntity, TProperty> : IFilterPredicate<TEntity>
    where TEntity : IEntity
{
    private readonly string _operator;
    private readonly string _property;
    private readonly IValueParser<TProperty> _valueParser;

    protected FilterPredicateBase(string property,
        string @operator,
        IValueParser<TProperty> valueParser)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(property);
        ArgumentException.ThrowIfNullOrWhiteSpace(@operator);
        ArgumentNullException.ThrowIfNull(valueParser);

        _property = property;
        _operator = @operator;
        _valueParser = valueParser;
    }

    public virtual bool Match(string property)
    {
        return _property.Equals(property, StringComparison.OrdinalIgnoreCase);
    }

    public virtual bool Match(string property, string @operator)
    {
        return _property.Equals(property, StringComparison.OrdinalIgnoreCase) &&
               _operator.Equals(@operator, StringComparison.OrdinalIgnoreCase);
    }

    public virtual bool CanParse(string comparisonValue, bool isLiteral)
    {
        return _valueParser.TryParse(comparisonValue, isLiteral, out _);
    }

    public virtual Expression<Func<TEntity, bool>> GetPredicate(string comparisonValue, bool isLiteral)
    {
        if (!_valueParser.TryParse(comparisonValue, isLiteral, out var value))
            throw new ArgumentException(
                "ComparisonValue is not parsable. Did you forget to call CanParse(...)?");

        return GetPredicate(value);
    }

    protected abstract Expression<Func<TEntity, bool>> GetPredicate(TProperty value);
}