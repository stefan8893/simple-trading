namespace SimpleTrading.DataAccess.PropertyFilterPredicates;

public interface IValueParser<out T>
{
    bool CanParse(string candidate, bool isLiteral);

    T Parse(string candidate, bool isLiteral);
}