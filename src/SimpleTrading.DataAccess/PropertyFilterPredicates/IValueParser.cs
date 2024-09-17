namespace SimpleTrading.DataAccess.PropertyFilterPredicates;

public interface IValueParser<T>
{
    bool TryParse(string candidate, bool isLiteral, out T result);
}