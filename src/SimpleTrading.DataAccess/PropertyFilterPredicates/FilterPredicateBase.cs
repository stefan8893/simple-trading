using System.Linq.Expressions;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates;

public abstract class FilterPredicateBase<TEntity, TProperty>(
    string property,
    string @operator,
    IValueParser<TProperty> valueParser)
    : IFilterPredicate<TEntity>
    where TEntity : IEntity
{
    private IValueParser<TProperty> ValueParser { get; } = valueParser;
    public string Property { get; } = property;
    public string Operator { get; } = @operator;

    public virtual bool Match(string property)
    {
        return !string.IsNullOrWhiteSpace(property) &&
               Property.Equals(property, StringComparison.OrdinalIgnoreCase);
    }

    public virtual bool Match(string property, string @operator)
    {
        if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(@operator))
            return false;

        return Property.Equals(property, StringComparison.OrdinalIgnoreCase) &&
               Operator.Equals(@operator, StringComparison.OrdinalIgnoreCase);
    }

    public virtual bool CanParse(string comparisonValue, bool isLiteral)
    {
        return ValueParser.TryParse(comparisonValue, isLiteral, out _);
    }

    public virtual Expression<Func<TEntity, bool>> GetPredicate(string comparisonValue, bool isLiteral)
    {
        if (!ValueParser.TryParse(comparisonValue, isLiteral, out var value))
            throw new ArgumentException(
                "ComparisonValue is not parsable. Did you forget to call CanParse(...)?");

        return GetPredicate(value);
    }

    protected abstract Expression<Func<TEntity, bool>> GetPredicate(TProperty value);
}